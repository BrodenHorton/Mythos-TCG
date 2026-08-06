using System;
using UnityEngine;

public class DomainFieldCardCollisionPointer : MonoBehaviour, FieldCardCollisionPointer {
    [SerializeField] private DomainFieldCardUI cardUI;

    public FieldCardUI GetFieldCardUI() {
        return cardUI;
    }

    public DomainFieldCardUI CardUI { get { return cardUI; } }
}
