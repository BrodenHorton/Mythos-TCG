public class CreatureDamagedByCreatureFinishedTrigger : EffectTrigger<CreatureCombatDamageEventArgs> {
    public override void Init() {
        EventBus.Instance.OnCreatureDamagedByCreatureFinished += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDamagedByCreatureFinished -= InvokeOnTriggerEffect;
    }
}