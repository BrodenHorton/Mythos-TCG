using System;

[Serializable]
public abstract class Card {
    protected Guid uuid;

    public Card() {
        uuid = Guid.NewGuid();
    }

    public abstract bool IsPlayable(DuelManager duelManager, DuelStateManager stateMaager, SpellChainManager spellChainManager, MatchPlayer player);

    public abstract void PlayCard(MatchPlayer player);

    public abstract void PlayCardFromHand(MatchPlayer player);

    public abstract int GetManaCost();

    public abstract CardPayload GetCardPayload();

    public Guid Uuid { get { return uuid; } }
}
