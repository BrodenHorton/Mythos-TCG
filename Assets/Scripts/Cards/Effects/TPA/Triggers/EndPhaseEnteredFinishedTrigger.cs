

public class EndPhaseEnteredFinishedTrigger : EffectTrigger<PlayerEventArgs> {
    private DuelStateManager stateManager;

    public EndPhaseEnteredFinishedTrigger() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public override void Init() {
        stateManager.EndPhase.OnEndPhasEnteredFinished += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        stateManager.EndPhase.OnEndPhasEnteredFinished -= InvokeOnTriggerEffect;
    }
}