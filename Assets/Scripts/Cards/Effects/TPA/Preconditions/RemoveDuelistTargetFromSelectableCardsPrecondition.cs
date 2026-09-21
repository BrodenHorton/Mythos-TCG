public class RemoveDuelistTargetFromSelectableCardsPrecondition : EffectPrecondition<DuelistContext, SelectableCardsEventArgs, CreatureCard> {
    public bool Evaluate(DuelistContext context, SelectableCardsEventArgs args) {
        if (context.DuelistDefender == null)
            return false;
        if (args.PlayerId != context.DuelistDefender.PlayerId)
            return false;
        if (!args.CardUuids.Contains(context.DuelistDefender.Uuid))
            return false;

        return true;
    }
}