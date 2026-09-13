using System;

public class CardSearchAction : EffectAction<CardSearchContext, EventArgs> {
    private DuelManager duelManager;

    public CardSearchAction() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public void Execute(CardSearchContext context, EventArgs _) {
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
