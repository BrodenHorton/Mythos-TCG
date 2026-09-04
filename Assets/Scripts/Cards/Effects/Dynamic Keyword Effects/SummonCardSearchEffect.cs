
public class SummonCardSearchEffect : SummonEffect {
    private SummonCardSearchEffectBase effectBase;
    private DuelManager duelManager;

    public SummonCardSearchEffect(SummonCardSearchEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        duelManager = ServiceLocator.Get<DuelManager>();

        EventBus.Instance.OnCreatureCardPlayedFromHand += SummonEffectHandler;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCardPlayedFromHand -= SummonEffectHandler;
    }

    protected override void SummonEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (args.PlayerId != card.PlayerId)
            return;
        if (args.Card.Uuid != card.Uuid)
            return;

        MatchPlayer player = duelManager.GetPlayerById(card.PlayerId);
        for (int i = 0; i < player.Deck.Count; i++) {
            Card deckCard = player.Deck[i];
            if (effectBase.TargetCard.Id.Equals(deckCard.GetCardBase().Id)) {
                TcgLogger.Log("SummonCardSearchEffect Proked");
                player.Deck.RemoveAt(i);
                player.AddCardToHand(deckCard);
                return;
            }
        }
    }

    public override string GetDynamicEffectDescription() {
        return "Add a " + effectBase.TargetCard.CardName + " card from your deck to your hand";
    }

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SummonCardSearchEffectPayload(this);
    }
}