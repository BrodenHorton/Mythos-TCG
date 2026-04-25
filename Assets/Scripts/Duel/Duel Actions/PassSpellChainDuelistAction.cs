
public class PassSpellChainDuelistAction : DuelistAction {
    private DuelManager duelManager;
    private SpellChainManager spellChainManager;

    public PassSpellChainDuelistAction(DuelManager duelManager, SpellChainManager spellChainManager) {
        activeActionMessage = "Pass";
        inactiveActionMessage = "Waiting for Opponent";
        this.duelManager = duelManager;
        this.spellChainManager = spellChainManager;

        spellChainManager.OnSpellAddedToSpellChain += ReomveAction;
    }

    public override void Execute() {
        spellChainManager.PassActionServerRpc();
    }

    private void ReomveAction(object sender, SpellCardAction action) {
        if (action.Initiator.PlayerId != duelManager.LocalClientPlayer.PlayerId)
            return;

        InvokeOnRemoveAction();
    }
}