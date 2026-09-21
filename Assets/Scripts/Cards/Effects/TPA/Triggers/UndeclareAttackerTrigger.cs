public class UndeclareAttackerTrigger : EffectTrigger<CombatCreatureEventArgs> {
    public override void Init() {
        EventBus.Instance.OnUndeclareAttacker += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnUndeclareAttacker -= InvokeOnTriggerEffect;
    }
}