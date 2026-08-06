public class SummonLifeGainEffect : SummonEffect {
    private SummonLifeGainEffectBase effectBase;
    private DuelManager duelManager;

    public SummonLifeGainEffect(SummonLifeGainEffectBase effectBase) {
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
        if (args.Card.Uuid != card.Uuid)
            return;

        duelManager.GetPlayerById(card.PlayerId).ModifyLifePoints(effectBase.LifePointsModifier);
    }

    public override string GetEffectDescription() {
        return "Increase life points by " + effectBase.LifePointsModifier;
    }

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SummonLifeGainEffectPayload(this);
    }
}
