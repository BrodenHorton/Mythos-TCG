
public class SkipDeclareSpellDuelistAction : DuelistAction {
    private DuelManager duelManager;
    private DeclareSpellsState declareSpellsState;

    public SkipDeclareSpellDuelistAction(DuelManager duelManager, SpellChainManager spellChainManager, DeclareSpellsState declareSpellsState) {
        activeActionMessage = "Skip";
        inactiveActionMessage = "Waiting for Opponent";
        this.duelManager = duelManager;
        this.declareSpellsState = declareSpellsState;

        spellChainManager.OnSpellChainStart += ReomveAction;
    }

    public override void Execute() {
        declareSpellsState.SkipActionServerRpc();
    }

    private void ReomveAction(object sender, PlayerEventArgs args) {
        if (args.Player.PlayerId != duelManager.LocalClientPlayer.PlayerId)
            return;

        TcgLogger.Log("Remove Action executed for SkipDeclareSpellDuelistAction");
        InvokeOnRemoveAction();
    }
}