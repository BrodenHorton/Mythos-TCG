public class SwarmCheckPrecondition : EffectPrecondition<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> {
    protected static readonly int SWARM_MINIMUM = 4;

    private DuelManager duelManager;

    public SwarmCheckPrecondition() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public bool Evaluate(SwarmAddEffectContext context, PlayerCardEventArgs<CreatureCard> args) {
        int swarmCount = 0;
        MatchPlayer player = duelManager.GetPlayerById(args.Card.PlayerId);
        // TODO: Change this to search through the new TPA effects for swarm or create a new event that swarm listens for
        /*foreach (CreatureCard creatureCard in player.Creatures) {
            if (creatureCard == args.Card)
                continue;

            foreach (CardEffect<CreatureCard> cardEffect in creatureCard.Effects) {
                if (cardEffect is SwarmEffect) {
                    swarmCount++;
                    if (swarmCount >= SWARM_MINIMUM)
                        return true;
                }
            }
        }*/

        return false;
    }
}
