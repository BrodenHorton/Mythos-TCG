public class CanCreatureAttackTrigger : EffectTrigger<PlayerCardCancelableEventArgs<CreatureCard>> {
    public override void Init() {
        EventBus.Instance.OnCanCreatureAttack += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCanCreatureAttack -= InvokeOnTriggerEffect;
    }
}