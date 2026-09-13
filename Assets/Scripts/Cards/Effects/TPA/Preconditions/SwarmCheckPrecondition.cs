public class SwarmCheckPrecondition : EffectPrecondition<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>> {
    protected static readonly int SWARM_MINIMUM = 4;

    private DuelManager duelManager;

    public SwarmCheckPrecondition() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public bool Evaluate(SwarmAddEffectContext context, PlayerCardEventArgs<CreatureCard> args) {
        int swarmCount = 0;
        MatchPlayer player = duelManager.GetPlayerById(args.Card.PlayerId);
        foreach (CreatureCard creatureCard in player.Creatures) {
            if (creatureCard == args.Card)
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
}
