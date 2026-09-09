
public class DeclareAttackersStateExitedTrigger : EffectTrigger<PlayerEventArgs> {
    private DuelStateManager stateManager; 

    public DeclareAttackersStateExitedTrigger() {
        stateManager = ServiceLocator.Get<DuelStateManager>();
    }

    public override void Init() {
        stateManager.EndPhase.OnEndPhasEnteredFinished += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        stateManager.EndPhase.OnEndPhasEnteredFinished -= InvokeOnTriggerEffect;
    }
}
