using UnityEngine;

public class CreatureFieldCardCollisionPointer : MonoBehaviour, CardCollisionPointer {
    [SerializeField] private CreatureFieldCardUI cardUI;

    public CardUI GetCardUI() {
        return cardUI;
    }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}
