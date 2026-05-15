
public class SkipDeclareSpellDuelistAction : DuelistAction {
    private DeclareSpellsState declareSpellsState;

    public SkipDeclareSpellDuelistAction(ulong playerId, SpellChainManager spellChainManager, DeclareSpellsState declareSpellsState) {
        this.playerId = playerId;
        activeActionMessage = "Skip";
        inactiveActionMessage = "Waiting for Opponent";
        this.declareSpellsState = declareSpellsState;

        spellChainManager.OnSpellChainStart += ReomveAction;
    }

    public override void Execute() {
        declareSpellsState.SkipAction();
    }

    private void ReomveAction(object sender, PlayerEventArgs args) {
        if (args.PlayerId != args.PlayerId)
            return;

        TcgLogger.Log("Remove Action executed for SkipDeclareSpellDuelistAction");
        InvokeOnRemoveAction();
    }
}