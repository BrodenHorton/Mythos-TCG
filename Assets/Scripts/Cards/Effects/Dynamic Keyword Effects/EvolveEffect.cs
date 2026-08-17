public class EvolveEffect : CreatureCardEffect {
    private static readonly string EVOLVE_KEYWORD = "evolve";

    private EvolveEffectBase effectBase;
    private int turnCount;
    private DuelManager duelManager;
    private DuelStateManager stateManager;

    public EvolveEffect(EvolveEffectBase effectBase) {
        this.effectBase = effectBase;
        turnCount = 0;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        duelManager = ServiceLocator.Get<DuelManager>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
        stateManager.StartPhase.OnStartPhaseEnteredFinished += EvolveEffectHandler;
    }

    public override void RemoveListeners() {
        stateManager.StartPhase.OnStartPhaseEnteredFinished -= EvolveEffectHandler;
    }

    private void EvolveEffectHandler(object sender, ulong currentPlayerTurnId) {
        if (currentPlayerTurnId != card.PlayerId)
            return;
        MatchPlayer player = duelManager.GetPlayerById(card.PlayerId);
        if (!player.Creatures.Contains(card))
            return;

        turnCount++;
        if(turnCount >= effectBase.EvolutionTurnCount) {
            card.DestroyCreature();
            player.PlayCard(effectBase.Evolution.GenerateCardFromBase(card.PlayerId));
        }
    }

    public override string GetRawDescription() {
        return CardRichTextUtil.GetKeywordLinkTagText(EVOLVE_KEYWORD, "Evolve " + effectBase.EvolutionTurnCount) + ": " + effectBase.Evolution.CardName;
    }

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new EvolveEffectPayload(this);
    }

    public int TurnCount { get { return turnCount; } }
}
