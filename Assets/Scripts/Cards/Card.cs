using System;
using UnityEngine;

[Serializable]
public abstract class Card {

    public abstract void DisplayCard();

    public abstract bool IsPlayable(DuelManager duelManager, MatchPlayer player);

    public abstract void PlayCard(DuelManager duelManager, MatchPlayer player);
}
