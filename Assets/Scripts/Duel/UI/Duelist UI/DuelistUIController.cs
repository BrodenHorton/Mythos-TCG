using UnityEngine;

public abstract class DuelistUIController : MonoBehaviour {    
    protected MatchPlayer player;

    public abstract void Init(MatchPlayer player);

    public abstract void SetLifePoints(int lifePoints);

    public abstract void SetManaCount(int manaCount);

    public abstract void DrawCard(Card card);

    public abstract void RemoveCardFromHand(int handIndex);

    public abstract DuelistUI GetDuelistUI();
}