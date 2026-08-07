public class DomainFieldCardUI : FieldCardUI {

    public void Init(ulong playerId, DomainCardPayload card) {
        cardUuid = card.Uuid;
        this.playerId = playerId;
    }

    public override void SelectCard(out bool canDragCard) {
        if (!isSelectable)
            throw new System.Exception("Attempting to call SelectCard when CardUI is not marked selectable");

        canDragCard = false;

    }

    public override void StartCardDrag() { }

    public override void ReleaseCardDrag() { }
}