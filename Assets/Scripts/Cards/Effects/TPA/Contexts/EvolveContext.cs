
public class EvolveContext : EffectContext<CreatureCard> {
    private CreatureCardBase evolution;
    private int evolutionRequiredTurnCount;
    private int currentTurnCount;

    public EvolveContext(string id, string effectName, CreatureCardBase evolution, int evolutionRequiredTurnCount) : base(id, effectName) {
        this.evolution = evolution;
        this.evolutionRequiredTurnCount = evolutionRequiredTurnCount;
    }

    public CreatureCardBase Evolution { get { return evolution; } }

    public int EvolutionRequiredTurnCount { get { return evolutionRequiredTurnCount; } }

    public int CurrentTurnCount { get {return currentTurnCount; } set { currentTurnCount = value; } }
}