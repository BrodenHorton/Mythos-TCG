public class SpellHandCardUI : HandCardUI {

    public void Init(SpellCardPayload card) {
        cardUuid = card.Uuid;
        cardName.text = card.CardBase.CardName;
        manaCost.text = card.ManaCost.ToString();
    }
}
