
public class DeathCryCardSearchEffect : DeathCryEffect {
    private DeathCryCardSearchEffectBase effectBase;
    private DuelManager duelManager;

    public DeathCryCardSearchEffect(DeathCryCardSearchEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        duelManager = ServiceLocator.Get<DuelManager>();

        EventBus.Instance.OnCreatureDestroyed += DeathCryEffectHandler;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDestroyed -= DeathCryEffectHandler;
    }

    protected override void DeathCryEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (args.PlayerId != card.PlayerId)
            return;
        if (args.Card.Uuid != card.Uuid)
            return;

        MatchPlayer player = duelManager.GetPlayerById(card.PlayerId);
        for (int i = 0; i < player.Deck.Count; i++) {
            Card deckCard = player.Deck[i];
            if (effectBase.TargetCard.Id.Equals(deckCard.GetCardBase().Id)) {
                TcgLogger.Log("DeathCryCardSearchEffect Proked");
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
        return new DeathCryCardSearchEffectPayload(this);
    }
}