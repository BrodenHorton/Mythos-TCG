public class CreatureDestroyedTrigger : EffectTrigger<PlayerCardEventArgs<CreatureCard>> {
    public override void Init() {
        EventBus.Instance.OnCreatureDestroyed += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDestroyed -= InvokeOnTriggerEffect;
    }
}
