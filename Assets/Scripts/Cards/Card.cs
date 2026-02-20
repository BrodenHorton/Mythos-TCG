using System;
using UnityEngine;

[Serializable]
public abstract class Card {
    private Guid uuid;

    public Card() {
        uuid = Guid.NewGuid();
    }

    public abstract void Init(MatchPlayer player);

    public abstract bool IsPlayable(DuelManager duelManager, MatchPlayer player);

    public abstract void PlayCard(DuelManager duelManager, MatchPlayer player);

    public Guid Uuid { get { return uuid; } }
}
