using System;

[Serializable]
public abstract class Card {
    protected Guid uuid;
    protected ulong playerId;

    public Card(ulong playerId) {
        uuid = Guid.NewGuid();
        this.playerId = playerId;
    }

    public abstract bool IsPlayable(DuelManager duelManager, DuelStateManager stateMaager, SpellChainManager spellChainManager, MatchPlayer player);

    public abstract void PlayCard(MatchPlayer player);

    public abstract void PlayCardFromHand(MatchPlayer player);

    public abstract int GetManaCost();

    public abstract CardPayload GetCardPayload();

    public Guid Uuid { get { return uuid; } }

    public ulong PlayerId { get { return playerId; } set { playerId = value; } }
}
