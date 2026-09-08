
#endregion

#region Triggers
public class LifePointsChangedTrigger : EffectTrigger<LifePointsChangedEventArgs> {
    public override void Init() {
        EventBus.Instance.OnLifePointsChanged += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnLifePointsChanged -= InvokeOnTriggerEffect;
    }
}
#endregion
