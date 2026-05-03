using UnityEngine;

public abstract class DuelistUIController : MonoBehaviour {    
    protected ulong playerId;

    public abstract void Init(ulong playerId, int lifePoints, int manaCount);

    public abstract void SetLifePoints(int lifePoints);

    public abstract void SetManaCount(int manaCount);

    public abstract void DrawCard(Card card);

    public abstract void RemoveCardFromHand(int handIndex);

    public abstract DuelistUI GetDuelistUI();
}