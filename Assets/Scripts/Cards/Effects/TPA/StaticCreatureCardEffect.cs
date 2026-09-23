public class StaticCreatureCardEffect : CardEffect<CreatureCard> {
    private string effectIconId;

    public StaticCreatureCardEffect(string effectName, string rawDescription, string effectIconId) : base(effectName, rawDescription) {
        this.effectIconId = effectIconId;
    }

    public string EffectIconId { get { return effectIconId; } }
}