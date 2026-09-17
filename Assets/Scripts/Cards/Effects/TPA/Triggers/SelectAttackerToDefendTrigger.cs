public class SelectAttackerToDefendTrigger : EffectTrigger<CanDefendEventArgs> {

    public override void Init() {
        EventBus.Instance.OnSelectAttackerToDefend += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectAttackerToDefend -= InvokeOnTriggerEffect;
    }
}
