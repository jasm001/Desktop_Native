using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ITSupportNative.Catalog.Application;
using ITSupportNative.Contracts.ControlPlane;
using ITSupportNative.Conversation.Application;
using ITSupportNative.Conversation.Domain;
using ITSupportNative.Desktop.ControlPlane;
using ITSupportNative.Desktop.Models;

namespace ITSupportNative.Desktop.ViewModels;

public sealed class AssistantViewModel : ObservableObject
{
    private readonly ConversationService _conversationService;
    private readonly IControlPlaneRequestClient _controlPlane;
    private ConversationSession _session;
    private int _commandSequence;
    private int _conversationSequence;
    private string _stateLabel = "Consulta";
    private string _response = "Elige una intención fija para iniciar la demostración.";
    private string _requestReference = "Sin solicitud creada";

    public AssistantViewModel(
        ConversationService conversationService,
        IControlPlaneRequestClient controlPlane)
    {
        _conversationService = conversationService;
        _controlPlane = controlPlane;
        _session = ConversationSession.Start("desktop-demo-0");

        QueryApprovedCommand = new RelayCommand(
            () => StartScenario(ConversationIntent.QueryCatalog, "aurora-code"));
        RequestApprovedCommand = new RelayCommand(
            () => StartScenario(ConversationIntent.RequestSoftware, "secure-transfer"));
        RequestProhibitedCommand = new RelayCommand(
            () => StartScenario(ConversationIntent.RequestSoftware, "share-anywhere"));
        ContinueCommand = new RelayCommand(
            ContinueProposal,
            () => _session.State == ConversationState.Proposal);
        ConfirmCommand = new AsyncRelayCommand(
            ConfirmRequestAsync,
            () => _session.State == ConversationState.ConfirmationRequired);
        CancelCommand = new RelayCommand(
            CancelRequest,
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

    public IRelayCommand QueryApprovedCommand { get; }

    public IRelayCommand RequestApprovedCommand { get; }

    public IRelayCommand RequestProhibitedCommand { get; }

    public IRelayCommand ContinueCommand { get; }

    public IAsyncRelayCommand ConfirmCommand { get; }

    public IRelayCommand CancelCommand { get; }

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

    private void StartScenario(ConversationIntent intent, string productReference)
    {
        _conversationSequence++;
        _session = ConversationSession.Start($"desktop-demo-{_conversationSequence}");

        Apply(
            _conversationService.Handle(
                _session,
                new ConversationCommand(
                    NextCommandId(),
                    intent,
                    productReference)));
    }

    private void ContinueProposal()
    {
        Apply(
            _conversationService.Handle(
                _session,
                new ConversationCommand(
                    NextCommandId(),
                    ConversationIntent.ContinueProposal)));
    }

    private async Task ConfirmRequestAsync()
    {
        ConversationTurn turn = _conversationService.Handle(
            _session,
            new ConversationCommand(
                NextCommandId(),
                ConversationIntent.Confirm));
        Apply(turn);

        SyntheticRequest? request = turn.Session.Request;
        if (turn.Code != ConversationResultCode.RequestCreated
            || request is null
            || request.Kind != ConversationRequestKind.SoftwareAcquisition)
        {
            return;
        }

        try
        {
            CreateSoftwareInstallationData? created =
                await _controlPlane.CreateSoftwareInstallationAsync(
                    request.IdempotencyKey,
                    request.ProductReference,
                    request.ProductVersion,
                    CancellationToken.None);
            if (created is null)
            {
                return;
            }

            RequestReference = $"Referencia local: {created.Request.Reference}";
            Response = created.Replayed
                ? "La API reutilizÃ³ la solicitud local existente sin duplicarla."
                : "La API persistiÃ³ la solicitud local; el agente simulado la procesarÃ¡ por el flujo tipado.";
        }
        catch (HttpRequestException)
        {
            Response =
                "La solicitud sintÃ©tica se conservÃ³, pero el control plane local no estÃ¡ disponible.";
        }
        catch (InvalidDataException)
        {
            Response =
                "La solicitud sintÃ©tica se conservÃ³, pero la respuesta local no fue vÃ¡lida.";
        }
    }

    private void CancelRequest()
    {
        Apply(
            _conversationService.Handle(
                _session,
                new ConversationCommand(
                    NextCommandId(),
                    ConversationIntent.Cancel)));
    }

    private void Apply(ConversationTurn turn)
    {
        _session = turn.Session;
        StateLabel = ToStateLabel(turn.Session.State);
        Response = ToResponse(turn);
        RequestReference = turn.Session.Request is null
            ? "Sin solicitud creada"
            : $"Referencia sintética: {turn.Session.Request.Reference}";

        ContinueCommand.NotifyCanExecuteChanged();
        ConfirmCommand.NotifyCanExecuteChanged();
        CancelCommand.NotifyCanExecuteChanged();
    }

    private string NextCommandId()
    {
        _commandSequence++;
        return $"desktop-command-{_commandSequence}";
    }

    private static string ToStateLabel(ConversationState state)
    {
        return state switch
        {
            ConversationState.Query => "Consulta",
            ConversationState.Proposal => "Propuesta",
            ConversationState.ConfirmationRequired => "Confirmación requerida",
            ConversationState.RequestCreated => "Solicitud creada",
            ConversationState.Cancelled => "Cancelada",
            _ => throw new InvalidOperationException("Estado de conversación no soportado."),
        };
    }

    private static string ToResponse(ConversationTurn turn)
    {
        return turn.Code switch
        {
            ConversationResultCode.QueryAnswered =>
                "Consulta completada. No se creó ninguna solicitud.",
            ConversationResultCode.ProposalReady =>
                ToProposalResponse(turn),
            ConversationResultCode.ConfirmationRequired =>
                "Confirma explícitamente para crear una referencia sintética o cancela.",
            ConversationResultCode.RequestCreated =>
                "Solicitud sintética creada una sola vez. No se ejecutó ninguna acción.",
            ConversationResultCode.Cancelled =>
                "La propuesta fue cancelada sin crear una solicitud.",
            ConversationResultCode.Rejected =>
                ToRejectedResponse(turn.CatalogDecision),
            ConversationResultCode.InvalidTransition =>
                "La opción no es válida para el estado actual.",
            ConversationResultCode.DuplicateCommand =>
                "El comando ya fue procesado; no se duplicó la solicitud.",
            _ => throw new InvalidOperationException("Resultado de conversación no soportado."),
        };
    }

    private static string ToProposalResponse(ConversationTurn turn)
    {
        return turn.Session.PendingRequest?.Kind == ConversationRequestKind.HumanReview
            ? "Se propone una revisión humana. Continúa para revisar la confirmación."
            : "Se propone solicitar el software aprobado. Continúa para confirmar.";
    }

    private static string ToRejectedResponse(CatalogDecision? decision)
    {
        string alternatives = decision?.Alternatives.Count > 0
            ? $" Alternativa aprobada: {string.Join(", ", decision.Alternatives.Select(product => product.Name))}."
            : string.Empty;

        return $"La solicitud fue rechazada por la regla de catálogo.{alternatives}";
    }
}
