public class WitherProkedTrigger : EffectTrigger<CreatureCombatDamageEventArgs> {
    public override void Init() {
        EventBus.Instance.OnWitherProked += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnWitherProked -= InvokeOnTriggerEffect;
    }
}