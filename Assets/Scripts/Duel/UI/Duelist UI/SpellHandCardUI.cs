public class SpellHandCardUI : HandCardUI {

    public void Init(SpellCard card) {
        cardUuid = card.Uuid;
        cardName.text = card.CardName;
        manaCost.text = card.GetManaCost().ToString();
    }
}
