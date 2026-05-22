using System;
using Unity.Netcode;
using UnityEngine;

public abstract class DuelistUIController : NetworkBehaviour {    
    protected ulong playerId;

    public abstract void Init(ulong playerId, int lifePoints, int manaCount);

    public abstract void SetLifePoints(int lifePoints);

    public abstract void SetManaCount(int manaCount);

    public abstract void DrawCard(CardPayload card);

    public abstract void RemoveCardFromHand(Guid cardUuid);

    public abstract DuelistUI GetDuelistUI();
}