public class StaticCreatureCardEffect : CardEffect<CreatureCard> {
    private string effectIconId;

    public StaticCreatureCardEffect(string rawDescription, string effectIconId) : base(rawDescription) {
        this.effectIconId = effectIconId;
    }

    public string EffectIconId { get { return effectIconId; } }
}