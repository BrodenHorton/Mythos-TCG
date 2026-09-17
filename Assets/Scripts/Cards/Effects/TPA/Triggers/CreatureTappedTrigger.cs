public class CreatureTappedTrigger : EffectTrigger<PlayerCardCancelableEventArgs<CreatureCard>> {

    public override void Init() {
        EventBus.Instance.OnCreatureTapped += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureTapped -= InvokeOnTriggerEffect;
    }
}
