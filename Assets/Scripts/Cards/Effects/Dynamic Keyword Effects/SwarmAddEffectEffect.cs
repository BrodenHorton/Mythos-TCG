public class SwarmAddEffectEffect : SwarmEffect {
    private SwarmAddEffectEffectBase effectBase;
    private CreatureCardEffect addedEffect;
    private DuelManager duelManager;

    public SwarmAddEffectEffect(SwarmAddEffectEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        duelManager = ServiceLocator.Get<DuelManager>();
        EventBus.Instance.OnCreatureCardPlayedFromHand += SwarmEffectHandler;
        EventBus.Instance.OnCreatureDestroyed += ClearSwarmEffect;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCardPlayedFromHand -= SwarmEffectHandler;
        EventBus.Instance.OnCreatureDestroyed -= ClearSwarmEffect;
    }

    protected override void SwarmEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (args.PlayerId != card.PlayerId)
            return;
        if (!CanActiveSwarm())
            return;
        if (addedEffect != null)
            return;

        card.AddEffect(effectBase.AddedEffectBase.GenerateCardEffectFromBase());
    }

    protected override void ClearSwarmEffect(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (args.PlayerId != card.PlayerId)
            return;
        if (CanActiveSwarm())
            return;
        if (addedEffect == null)
            return;

        card.RemoveEffect(addedEffect);
    }

    private bool CanActiveSwarm() {
        int swarmCount = 0;
        MatchPlayer player = duelManager.GetPlayerById(card.PlayerId);
        foreach (CreatureCard creatureCard in player.Creatures) {
            if (creatureCard == card)
                continue;

            foreach (CreatureCardEffect cardEffect in creatureCard.Effects) {
                if (cardEffect is SwarmEffect) {
                    swarmCount++;
                    if (swarmCount >= SWARM_MINIMUM)
                        return true;
                }
            }
        }

        return false;
    }

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return effectBase;
    }

    public override string GetDynamicEffectDescription() {
        return "Aquire the effect " + effectBase.AddedEffectBase.EffectName;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SwarmAddEffectEffectPayload(this);
    }
}