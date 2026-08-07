
public abstract class FieldCardUI : CardUI {

    protected override void Start() {
        base.Start();
        EventBus.Instance.OnPostCreatureDestroyed += DestroyFieldCardUI;
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        EventBus.Instance.OnPostCreatureDestroyed -= DestroyFieldCardUI;
    }

    private void DestroyFieldCardUI(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (args.CardPayload.Uuid != cardUuid)
            return;

        CardSelectionManager.Instance.OnSetSelectableCards -= SetSelectabilityOnSetSelectableCards;
        EventBus.Instance.OnPostCreatureDestroyed -= DestroyFieldCardUI;
        Destroy(gameObject);
    }
}
