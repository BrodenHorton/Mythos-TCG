using Unity.Netcode;

public class NullCard : Card {

    public NullCard(ulong playerId) : base(playerId) { }

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, SpellChainManager spellChainManager, MatchPlayer player) {
        return false;
    }

    public override void PlayCard(MatchPlayer player) { }

    public override void PlayCardFromHand(MatchPlayer player) { }

    public override int GetManaCost() {
        return 999;
    }

    public override CardPayload GetCardPayload() {
        return new NullCardPayload();
    }
}