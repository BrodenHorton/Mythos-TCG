public class CreatureCombatFinishedTrigger : EffectTrigger<CreatureCombatDamageEventArgs> {

    public override void Init() {
        EventBus.Instance.OnCreatureCombatFinished += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCombatFinished -= InvokeOnTriggerEffect;
    }
}