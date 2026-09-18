public class SummoningSicknessTrigger : EffectTrigger<PlayerCardCancelableEventArgs<CreatureCard>> {

    public override void Init() {
        EventBus.Instance.OnSummoningSickness += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSummoningSickness -= InvokeOnTriggerEffect;
    }
}