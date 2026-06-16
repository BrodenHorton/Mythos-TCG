public class SpellFieldCardUI : FieldCardUI {

    public void Init(ulong playerId, SpellCard card) {
        cardUuid = card.Uuid;
        this.playerId = playerId;
    }
}
