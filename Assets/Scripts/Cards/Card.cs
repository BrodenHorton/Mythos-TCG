using System;
using UnityEngine;

[Serializable]
public abstract class Card {

    public abstract void DisplayCard();

    public abstract bool IsPlayable();
}
