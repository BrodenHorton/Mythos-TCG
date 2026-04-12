public class DomainHandCardUI : HandCardUI {

    public void Init(DomainCard card) {
        cardUuid = card.Uuid;
        cardName.text = card.CardName;
        manaCost.text = card.GetManaCost().ToString();
    }
}
