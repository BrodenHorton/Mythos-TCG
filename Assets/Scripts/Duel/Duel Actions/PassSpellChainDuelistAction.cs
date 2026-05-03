
public class PassSpellChainDuelistAction : DuelistAction {
    private DuelManager duelManager;
    private SpellChainManager spellChainManager;

    public PassSpellChainDuelistAction(ulong playerId, DuelManager duelManager, SpellChainManager spellChainManager) {
        this.playerId = playerId;
        activeActionMessage = "Pass";
        inactiveActionMessage = "Waiting for Opponent";
        this.duelManager = duelManager;
        this.spellChainManager = spellChainManager;

        spellChainManager.OnSpellChainTurnEnd += ReomveAction;
    }

    public override void Execute() {
        spellChainManager.PassActionServerRpc();
    }

    private void ReomveAction(object sender, PlayerEventArgs args) {
        if (args.Player.PlayerId != duelManager.LocalClientPlayer.PlayerId)
            return;

        InvokeOnRemoveAction();
    }
}
