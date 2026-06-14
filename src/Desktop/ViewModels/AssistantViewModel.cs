using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Conversation.Domain;
using ITSupportNative.Desktop.Conversation;
using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class AssistantViewModel : ObservableObject
{
    private readonly ConversationChannelService _channelService;
    private ConversationSession _session;
    private int _commandSequence;
    private int _conversationSequence;
    private string _stateLabel = "Consulta";
    private string _response =
        "Elige una intención fija para iniciar la demostración.";
    private string _requestReference = "Sin solicitud creada";
    private ConversationChannelOutput? _lastChannelOutput;

    public AssistantViewModel(ConversationChannelService channelService)
    {
        _channelService = channelService;
        _session = ConversationSession.Start("desktop-demo-0");

        QueryApprovedCommand = new AsyncRelayCommand(
            () => StartScenarioAsync(
                ConversationChannelActions.CatalogQuery,
                "aurora-code"));
        RequestApprovedCommand = new AsyncRelayCommand(
            () => StartScenarioAsync(
                ConversationChannelActions.SoftwareRequest,
                "secure-transfer"));
        RequestProhibitedCommand = new AsyncRelayCommand(
            () => StartScenarioAsync(
                ConversationChannelActions.SoftwareRequest,
                "share-anywhere"));
        ContinueCommand = new AsyncRelayCommand(
            () => RunAsync(ConversationChannelActions.ProposalContinue),
            () => _session.State == ConversationState.Proposal);
        ConfirmCommand = new AsyncRelayCommand(
            () => RunAsync(ConversationChannelActions.RequestConfirm),
            () => _session.State == ConversationState.ConfirmationRequired);
        CancelCommand = new AsyncRelayCommand(
            () => RunAsync(ConversationChannelActions.ConversationCancel),
            () => _session.State is ConversationState.Proposal
                or ConversationState.ConfirmationRequired);

        Suggestions =
        [
            new(
                "Consultar software aprobado",
                "Consulta Aurora Code Editor sin crear una solicitud.",
                "\uE721",
                QueryApprovedCommand),
            new(
                "Solicitar software aprobado",
                "Prepara una propuesta y exige confirmación explícita.",
                "\uE8A5",
                RequestApprovedCommand),
            new(
                "Solicitar software prohibido",
                "Demuestra el rechazo y las alternativas aprobadas.",
                "\uE783",
                RequestProhibitedCommand),
        ];
    }

    public IReadOnlyList<AssistantSuggestion> Suggestions { get; }

    public IAsyncRelayCommand QueryApprovedCommand { get; }

    public IAsyncRelayCommand RequestApprovedCommand { get; }

    public IAsyncRelayCommand RequestProhibitedCommand { get; }

    public IAsyncRelayCommand ContinueCommand { get; }

    public IAsyncRelayCommand ConfirmCommand { get; }

    public IAsyncRelayCommand CancelCommand { get; }

    public string StateLabel
    {
        get => _stateLabel;
        private set => SetProperty(ref _stateLabel, value);
    }

    public string Response
    {
        get => _response;
        private set => SetProperty(ref _response, value);
    }

    public string RequestReference
    {
        get => _requestReference;
        private set => SetProperty(ref _requestReference, value);
    }

    public ConversationChannelOutput? LastChannelOutput
    {
        get => _lastChannelOutput;
        private set => SetProperty(ref _lastChannelOutput, value);
    }

    private async Task StartScenarioAsync(
        string action,
        string productReference)
    {
        _conversationSequence++;
        _session = ConversationSession.Start(
            $"desktop-demo-{_conversationSequence}");
        await RunAsync(action, productReference);
    }

    private async Task RunAsync(
        string action,
        string? productReference = null)
    {
        string messageId = NextCommandId();
        ConversationChannelInput input = WinUiConversationInputFactory.Create(
            _session,
            messageId,
            $"desktop-{messageId}",
            action,
            productReference);
        ConversationChannelTurn turn = await _channelService.HandleAsync(
            _session,
            input,
            CancellationToken.None);

        Apply(turn);
    }

    private void Apply(ConversationChannelTurn turn)
    {
        _session = turn.Session;
        LastChannelOutput = turn.Output;
        StateLabel = ToStateLabel(turn.Output.State);
        Response = ToResponse(turn.Output);
        RequestReference = ToRequestReference(turn.Output.Request);

        ContinueCommand.NotifyCanExecuteChanged();
        ConfirmCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    private string NextCommandId()
    {
        _commandSequence++;
        return $"desktop-command-{_commandSequence}";
    }

    private static string ToStateLabel(string state)
    {
        return state switch
        {
            "query" => "Consulta",
            "proposal" => "Propuesta",
            "confirmation_required" => "Confirmación requerida",
            "request_created" => "Solicitud creada",
            "cancelled" => "Cancelada",
            _ => throw new InvalidOperationException(
                "Estado de conversación no soportado."),
        };
    }

    private static string ToResponse(ConversationChannelOutput output)
    {
        return output.ResultCode switch
        {
            "query_answered" =>
                "Consulta completada. No se creó ninguna solicitud.",
            "proposal_ready" => output.Decision?.Kind == "escalate"
                ? "Se propone una revisión humana. Continúa para revisar la confirmación."
                : "Se propone solicitar el software aprobado. Continúa para confirmar.",
            "confirmation_required" =>
                "Confirma explícitamente para crear una referencia sintética o cancela.",
            "request_created" => ToCreatedResponse(output.Request),
            "cancelled" =>
                "La propuesta fue cancelada sin crear una solicitud.",
            "rejected" => ToRejectedResponse(output.Decision),
            "invalid_transition" =>
                "La opción no es válida para el estado actual.",
            "duplicate_command" =>
                "El comando ya fue procesado; no se duplicó la solicitud.",
            "capability_unavailable" =>
                "La confirmación fue validada, pero la revisión humana durable aún no está disponible.",
            "control_plane_unavailable" =>
                "La solicitud sintética se conservó, pero el control plane local no está disponible.",
            _ => throw new InvalidOperationException(
                "Resultado de conversación no soportado."),
        };
    }

    private static string ToCreatedResponse(
        ConversationChannelRequestView? request)
    {
        if (request?.PersistedRequestId is null)
        {
            return "Solicitud sintética creada una sola vez. No se ejecutó ninguna acción.";
        }

        return request.Replayed == true
            ? "La API reutilizó la solicitud local existente sin duplicarla."
            : "La API persistió la solicitud local; el agente simulado la procesará por el flujo tipado.";
    }

    private static string ToRejectedResponse(
        ConversationChannelDecision? decision)
    {
        string alternatives = decision?.Alternatives.Count > 0
            ? $" Alternativa aprobada: {string.Join(
                ", ",
                decision.Alternatives.Select(product => product.ProductName))}."
            : string.Empty;

        return $"La solicitud fue rechazada por la regla de catálogo.{alternatives}";
    }

    private static string ToRequestReference(
        ConversationChannelRequestView? request)
    {
        if (request?.PersistedReference is not null)
        {
            return $"Referencia local: {request.PersistedReference}";
        }

        return request is null
            ? "Sin solicitud creada"
            : $"Referencia sintética: {request.SyntheticReference}";
    }
}
