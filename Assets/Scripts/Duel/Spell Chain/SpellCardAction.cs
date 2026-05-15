public class SpellCardAction {
    private SpellCard card;
    private ulong initiatorId;

    public SpellCardAction(SpellCard card, ulong initiatorId) {
        this.card = card;
        this.initiatorId = initiatorId;
    }

    public SpellCard Card { get { return card; } }

    public ulong InitiatorId { get { return initiatorId; } }
}