public class DomainFieldCardUI : FieldCardUI {

    public void Init(DomainCardPayload card) {
        cardUuid = card.Uuid;
    }
}