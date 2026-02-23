using UnityEngine;

public abstract class DuelistUIController : MonoBehaviour {    
    protected MatchPlayer player;

    public void Init(MatchPlayer player) {
        this.player = player;
    }

    public abstract DuelistUI GetDuelistUI();
}