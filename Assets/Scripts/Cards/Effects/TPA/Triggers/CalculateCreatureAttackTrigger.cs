
public class CalculateCreatureAttackTrigger : EffectTrigger<PlayerCardStatEventArgs<CreatureCard>> {
    public override void Init() {
        EventBus.Instance.OnCalculateCreatureAttack += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCalculateCreatureAttack -= InvokeOnTriggerEffect;
    }
}
