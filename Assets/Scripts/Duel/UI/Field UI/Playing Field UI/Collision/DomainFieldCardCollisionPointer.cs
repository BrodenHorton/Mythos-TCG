using System;
using UnityEngine;

public class DomainFieldCardCollisionPointer : MonoBehaviour, CardCollisionPointer {
    [SerializeField] private DomainFieldCardUI cardUI;

    public CardUI GetCardUI() {
        return cardUI;
    }

    public DomainFieldCardUI CardUI { get { return cardUI; } }
}
