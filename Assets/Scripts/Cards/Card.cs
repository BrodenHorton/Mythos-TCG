using System;

[Serializable]
public abstract class Card {
    protected Guid uuid;

    public Card() {
        uuid = Guid.NewGuid();
    }

    public abstract bool IsPlayable(DuelManager duelManager, MatchPlayer player);

    public abstract void PlayCard(DuelManager duelManager, MatchPlayer player);

    public abstract void PlayCardFromHand(DuelManager duelManager, MatchPlayer player, int handIndex);

    public abstract int GetManaCost();

    public Guid Uuid { get { return uuid; } }
}
