public class DomainHandCardUI : HandCardUI {

    public void Init(DomainCardPayload card, ulong playerId) {
        this.playerId = playerId;
        cardUuid = card.Uuid;
        cardName.text = card.CardBase.CardName;
        manaCost.text = card.ManaCost.ToString();
        AddListeners();
    }
}
