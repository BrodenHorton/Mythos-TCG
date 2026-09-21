public class GetSelectableCardsTrigger : EffectTrigger<SelectableCardsEventArgs> {
    public override void Init() {
        CardSelectionManager.Instance.OnGetSelectableCards += InvokeOnTriggerEffect;
    }

    public override void RemoveListeners() {
        CardSelectionManager.Instance.OnGetSelectableCards -= InvokeOnTriggerEffect;
    }
}
