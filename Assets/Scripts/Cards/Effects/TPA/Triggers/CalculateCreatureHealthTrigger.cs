

public class CalculateCreatureHealthTrigger : EffectTrigger<PlayerCardStatEventArgs<CreatureCard>> {
    public override void Init() {
        EventBus.Instance.OnCalculateCreatureHealth += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCalculateCreatureHealth -= InvokeOnTriggerEffect;
    }
}