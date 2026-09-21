
public class CreatureReleasedOverCreatureTrigger : EffectTrigger<CreatureReleasedOverCreatureEventArgs> {
    public override void Init() {
        EventBus.Instance.OnCreatureReleasedOverCreature += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureReleasedOverCreature -= InvokeOnTriggerEffect;
    }
}