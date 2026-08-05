using System;
using UnityEngine;

public class CreatureFieldCardCollisionPointer : MonoBehaviour, FieldCardCollisionPointer {
    [SerializeField] private CreatureFieldCardUI cardUI;

    public FieldCardUI GetFieldCardUI() {
        return cardUI;
    }

    public CardUI GetCardUI() {
        throw new NotImplementedException();
    }

    public CreatureFieldCardUI CardUI { get { return cardUI; } }
}
