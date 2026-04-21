public class SpellCardAction {
    private SpellCard card;
    private MatchPlayer initiator;

    public SpellCardAction(SpellCard card, MatchPlayer initiator) {
        this.card = card;
        this.initiator = initiator;
    }

    public SpellCard Card { get { return card; } }

    public MatchPlayer Initiator { get { return initiator; } }
}