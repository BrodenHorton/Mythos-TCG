public class SummonTrigger : EffectTrigger<PlayerCardEventArgs<CreatureCard>> {

    public override void Init() {
        EventBus.Instance.OnCreatureCardPlayedFromHand += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCardPlayedFromHand -= InvokeOnTriggerEffect;
    }
}
