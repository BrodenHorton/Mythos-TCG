using UnityEngine;

public class CreatureFieldCardCollisionPointer : MonoBehaviour, FieldCardCollisionPointer {
    [SerializeField] private CreatureFieldCardUI cardUI;

    public FieldCardUI GetFieldCardUI() {
        return cardUI;
    }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}
