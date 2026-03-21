public class SpellHandCardUI : HandCardUI {

    public void Init(SpellCard card) {
        cardName.text = card.CardName;
        manaCost.text = card.GetManaCost().ToString();
    }
}
