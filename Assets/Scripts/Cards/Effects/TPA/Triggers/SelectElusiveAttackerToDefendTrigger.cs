public class SelectElusiveAttackerToDefendTrigger : EffectTrigger<CanDefendEventArgs> {

    public override void Init() {
        EventBus.Instance.OnSelectElusiveAttackerToDefend += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectElusiveAttackerToDefend -= InvokeOnTriggerEffect;
    }
}