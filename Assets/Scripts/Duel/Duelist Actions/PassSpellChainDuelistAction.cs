
public class PassSpellChainDuelistAction : DuelistAction {
    private SpellChainManager spellChainManager;

    public PassSpellChainDuelistAction(ulong playerId, SpellChainManager spellChainManager) {
        this.playerId = playerId;
        activeActionMessage = "Pass";
        inactiveActionMessage = "Waiting for Opponent";
        this.spellChainManager = spellChainManager;

        spellChainManager.OnSpellChainTurnEnd += ReomveAction;
    }

    public override void Execute() {
        spellChainManager.PassAction();
    }

    private void ReomveAction(object sender, PlayerEventArgs args) {
        if (args.PlayerId != playerId)
            return;

        InvokeOnRemoveAction();
    }
}
