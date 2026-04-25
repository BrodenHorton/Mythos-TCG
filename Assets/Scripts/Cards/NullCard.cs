public class NullCard : Card {

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, SpellChainManager spellChainManager, MatchPlayer player) {
        return false;
    }

    public override void PlayCard(MatchPlayer player) { }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) { }

    public override int GetManaCost() {
        return 999;
    }
}