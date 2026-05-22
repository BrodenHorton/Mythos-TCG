public class DomainHandCardUI : HandCardUI {

    public void Init(DomainCardPayload card) {
        cardUuid = card.Uuid;
        cardName.text = card.CardBase.CardName;
        manaCost.text = card.ManaCost.ToString();
    }
}
