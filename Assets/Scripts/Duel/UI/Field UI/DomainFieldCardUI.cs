public class DomainFieldCardUI : FieldCardUI {

    public void Init(ulong playerId, DomainCardPayload card) {
        cardUuid = card.Uuid;
        this.playerId = playerId;
    }
}