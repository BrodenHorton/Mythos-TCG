using UnityEngine;

public class HandCardCollisionPointer : MonoBehaviour, CardCollisionPointer {
    [SerializeField] private HandCardUI handCardUI;

    public CardUI GetCardUI() {
        return handCardUI;
    }

    public HandCardUI HandCardUI { get { return handCardUI; } }
}
