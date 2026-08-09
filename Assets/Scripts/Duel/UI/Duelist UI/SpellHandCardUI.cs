public class SpellHandCardUI : HandCardUI {

    public void Init(SpellCardPayload card, ulong playerId) {
        this.playerId = playerId;
        cardUuid = card.Uuid;
        cardName.text = card.CardBase.CardName;
        manaCost.text = card.ManaCost.ToString();
        AddListeners();
    }
}
