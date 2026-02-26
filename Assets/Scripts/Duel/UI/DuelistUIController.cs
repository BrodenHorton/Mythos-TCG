using UnityEngine;

public abstract class DuelistUIController : MonoBehaviour {    
    protected MatchPlayer player;

    public abstract void Init(MatchPlayer player);

    public abstract DuelistUI GetDuelistUI();
}