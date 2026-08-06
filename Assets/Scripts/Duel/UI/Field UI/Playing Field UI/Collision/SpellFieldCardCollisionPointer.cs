using System;
using UnityEngine;

public class SpellFieldCardCollisionPointer : MonoBehaviour, FieldCardCollisionPointer {
    [SerializeField] private SpellFieldCardUI cardUI;

    public FieldCardUI GetFieldCardUI() {
        return cardUI;
    }

    public SpellFieldCardUI CardUI { get { return cardUI; } }
}
