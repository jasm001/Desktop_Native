using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITSupportNative.Contracts.Conversation;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Conversation.Domain;
using ITSupportNative.Desktop.Assistant;
using ITSupportNative.Desktop.Conversation;
using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class AssistantViewModel : ObservableObject
{
    private readonly ConversationChannelService _channelService;
    private readonly IAssistantProvider _assistantProvider;
    private ConversationSession _session;
    private int _commandSequence;
    private int _conversationSequence;
    private string _draftMessage = string.Empty;
    private string _stateLabel = "Consulta";
    private string _response =
        "Elige una intencion fija o escribe una consulta si Hermes local esta habilitado.";
    private string _requestReference = "Sin solicitud creada";
    private bool _isWaitingForAssistant;
    private ConversationChannelOutput? _lastChannelOutput;

    public AssistantViewModel(
        ConversationChannelService channelService,
        IAssistantProvider assistantProvider)
    {
        _channelService = channelService;
        _assistantProvider = assistantProvider;
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
            () => RunUserActionAsync(ConversationChannelActions.ProposalContinue),
            () => _session.State == ConversationState.Proposal);
        ConfirmCommand = new AsyncRelayCommand(
            () => RunUserActionAsync(ConversationChannelActions.RequestConfirm),
            () => _session.State == ConversationState.ConfirmationRequired);
        CancelCommand = new AsyncRelayCommand(
            () => RunUserActionAsync(ConversationChannelActions.ConversationCancel),
            () => _session.State is ConversationState.Proposal
                or ConversationState.ConfirmationRequired);
        SendMessageCommand = new AsyncRelayCommand(
            SendMessageAsync,
            CanSendMessage);

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

        ChatMessages =
        [
            AssistantMessage(
                "Asistente",
                "Elige una opcion fija o escribe una consulta para Hermes local.",
                "Consulta informativa; sin solicitud ni accion creada"),
        ];
    }

    public IReadOnlyList<AssistantSuggestion> Suggestions { get; }

    public ObservableCollection<AssistantChatMessage> ChatMessages { get; }

    public IAsyncRelayCommand QueryApprovedCommand { get; }

    public IAsyncRelayCommand RequestApprovedCommand { get; }

    public IAsyncRelayCommand RequestProhibitedCommand { get; }

    public IAsyncRelayCommand ContinueCommand { get; }

    public IAsyncRelayCommand ConfirmCommand { get; }

    public IAsyncRelayCommand CancelCommand { get; }

    public IAsyncRelayCommand SendMessageCommand { get; }

    public bool IsFreeTextEnabled => _assistantProvider.IsAvailable;

    public bool IsWaitingForAssistant
    {
        get => _isWaitingForAssistant;
        private set
        {
            if (SetProperty(ref _isWaitingForAssistant, value))
            {
                SendMessageCommand.NotifyCanExecuteChanged();
            }
        }
    }

    public string FreeTextPlaceholder => _assistantProvider.IsAvailable
        ? "Escribe una consulta para Hermes local"
        : "Hermes local no esta configurado";

    public string DraftMessage
    {
        get => _draftMessage;
        set
        {
            if (SetProperty(ref _draftMessage, value))
            {
                SendMessageCommand.NotifyCanExecuteChanged();
            }
        }
    }

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
        AddUserMessage(ToSuggestionTitle(action, productReference));
        _conversationSequence++;
        _session = ConversationSession.Start(
            $"desktop-demo-{_conversationSequence}");
        await RunAsync(action, productReference);
    }

    private async Task RunUserActionAsync(string action)
    {
        AddUserMessage(ToConversationActionTitle(action));
        await RunAsync(action);
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

    private bool CanSendMessage()
    {
        return _assistantProvider.IsAvailable
            && !IsWaitingForAssistant
            && !string.IsNullOrWhiteSpace(DraftMessage);
    }

    private async Task SendMessageAsync()
    {
        string message = DraftMessage.Trim();
        DraftMessage = string.Empty;
        AddUserMessage(message);
        StateLabel = "Hermes local";
        Response = "Consultando Hermes local...";
        RequestReference = "Consulta informativa; sin solicitud ni accion creada";
        IsWaitingForAssistant = true;

        try
        {
            AssistantProviderReply reply =
                await _assistantProvider.GetResponseAsync(
                    message,
                    CancellationToken.None);
            Response = reply.Text;
            RequestReference =
                $"Fuente: {reply.Source}; sin solicitud ni accion creada";
            ChatMessages.Add(
                AssistantMessage(
                    "Hermes local",
                    reply.Text,
                    RequestReference));
        }
        catch (OperationCanceledException)
        {
            Response =
                "Hermes local no respondio dentro del limite. El asistente conserva las opciones deterministas.";
            RequestReference = "Sin solicitud creada";
            ChatMessages.Add(
                AssistantMessage(
                    "Hermes local",
                    Response,
                    "Sin solicitud ni accion creada"));
        }
        catch (HttpRequestException)
        {
            Response =
                "Hermes local no esta disponible. El asistente conserva las opciones deterministas.";
            RequestReference = "Sin solicitud creada";
            ChatMessages.Add(
                AssistantMessage(
                    "Hermes local",
                    Response,
                    "Sin solicitud ni accion creada"));
        }
        catch (InvalidOperationException)
        {
            Response =
                "Hermes local devolvio una respuesta no utilizable. El asistente conserva las opciones deterministas.";
            RequestReference = "Sin solicitud creada";
            ChatMessages.Add(
                AssistantMessage(
                    "Hermes local",
                    Response,
                    "Sin solicitud ni accion creada"));
        }
        finally
        {
            IsWaitingForAssistant = false;
        }
    }

    private void Apply(ConversationChannelTurn turn)
    {
        _session = turn.Session;
        LastChannelOutput = turn.Output;
        StateLabel = ToStateLabel(turn.Output.State);
        Response = ToResponse(turn.Output);
        RequestReference = ToRequestReference(turn.Output.Request);
        ChatMessages.Add(
            AssistantMessage(StateLabel, Response, RequestReference));

        ContinueCommand.NotifyCanExecuteChanged();
        ConfirmCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    private string NextCommandId()
    {
        _commandSequence++;
        return $"desktop-command-{_commandSequence}";
    }

    private void AddUserMessage(string text)
    {
        ChatMessages.Add(
            new AssistantChatMessage(
                "Tu",
                text,
                "Entrada local; sin solicitud creada por la consulta",
                "\uE13D",
                Microsoft.UI.Xaml.HorizontalAlignment.Right));
    }

    private static AssistantChatMessage AssistantMessage(
        string author,
        string body,
        string detail)
    {
        return new(
            author,
            body,
            detail,
            "\uE8BD",
            Microsoft.UI.Xaml.HorizontalAlignment.Left);
    }

    private static string ToSuggestionTitle(
        string action,
        string productReference)
    {
        return action switch
        {
            ConversationChannelActions.CatalogQuery =>
                $"Consultar {productReference}",
            ConversationChannelActions.SoftwareRequest =>
                $"Solicitar {productReference}",
            _ => "Continuar conversacion",
        };
    }

    private static string ToConversationActionTitle(string action)
    {
        return action switch
        {
            ConversationChannelActions.ProposalContinue =>
                "Continuar propuesta",
            ConversationChannelActions.RequestConfirm =>
                "Confirmar solicitud sintetica",
            ConversationChannelActions.ConversationCancel =>
                "Cancelar propuesta",
            _ => "Continuar conversacion",
        };
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
