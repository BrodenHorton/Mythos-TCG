public class StartPhaseEnteredFinishedTrigger : EffectTrigger<PlayerEventArgs> {
    private DuelStateManager stateManager;

    public StartPhaseEnteredFinishedTrigger() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public override void Init() {
        stateManager.StartPhase.OnStartPhaseEnteredFinished += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        stateManager.StartPhase.OnStartPhaseEnteredFinished -= InvokeOnTriggerEffect;
    }
}