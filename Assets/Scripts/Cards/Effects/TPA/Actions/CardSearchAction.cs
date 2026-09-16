using System;

public class CardSearchAction<TCard> : EffectAction<CardSearchContext<TCard>, EventArgs, TCard> where TCard : Card {
    private DuelManager duelManager;

    public CardSearchAction() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public void Execute(CardSearchContext<TCard> context, EventArgs args) {
        MatchPlayer player = duelManager.GetPlayerById(context.Card.PlayerId);
        for (int i = 0; i < player.Deck.Count; i++) {
            Card deckCard = player.Deck[i];
            if (context.SearchTarget.Id.Equals(deckCard.GetCardBase().Id)) {
                TcgLogger.Log("CardSearchAction Proked");
                player.Deck.RemoveAt(i);
                player.AddCardToHand(deckCard);
                return;
            }
        }
    }
}
