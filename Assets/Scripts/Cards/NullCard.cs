public class NullCard : Card {

    public override int GetManaCost() {
        return 999;
    }

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
        return false;
    }

    public override void PlayCard(DuelManager duelManager, MatchPlayer player) {}

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {}
}