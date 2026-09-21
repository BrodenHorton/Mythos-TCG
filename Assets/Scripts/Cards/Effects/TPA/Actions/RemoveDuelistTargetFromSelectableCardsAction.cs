public class RemoveDuelistTargetFromSelectableCardsAction : EffectAction<DuelistContext, SelectableCardsEventArgs, CreatureCard> {
    public void Execute(DuelistContext context, SelectableCardsEventArgs args) {
        TcgLogger.Log("Duelist Remove Defender from Selectable Cards Activated");
        args.CardUuids.Remove(context.DuelistDefender.Uuid);
    }
}
