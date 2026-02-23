public class SpellHandCardUI : HandCardUI {

    public void Init(SpellCard card) {
        manaCost.text = card.GetManaCost().ToString();
    }
}
