public class CreatureDamagedByCreatureTrigger : EffectTrigger<CreatureCombatDamageEventArgs> {
    public override void Init() {
        EventBus.Instance.OnCreatureDamagedByCreature += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDamagedByCreature -= InvokeOnTriggerEffect;
    }
}