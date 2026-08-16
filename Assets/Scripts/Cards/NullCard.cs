using System;

public class NullCard : Card {

    public NullCard(ulong playerId) : base(playerId) { }

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, SpellChainManager spellChainManager, MatchPlayer player) {
        throw new Exception("Attempting to call IsPlayable on a Null Card");
    }

    public override int GetManaCost() {
        throw new Exception("Attempting to get mana count from Null card");
    }

    public override CardBase GetCardBase() {
        throw new Exception("Attempting to get card base from Null card");
    }

    public override CardPayload GetCardPayload() {
        return new NullCardPayload();
    }
}