public class NullCard : Card {

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
        return false;
    }

    public override void PlayCard(DuelManager duelManager, MatchPlayer player) { }

    public override void PlayCardFromHand(DuelManager duelManager, MatchPlayer player, int handIndex) { }

    public override int GetManaCost() {
        return 999;
    }
}