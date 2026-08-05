using System;
using UnityEngine;

public class DomainFieldCardCollisionPointer : MonoBehaviour, FieldCardCollisionPointer {
    [SerializeField] private DomainFieldCardUI cardUI;

    public Guid GetCardUuid() {
        return cardUI.CardUuid;
    }

    public ulong GetPlayerId() {
        return cardUI.PlayerId;
    }

    public FieldCardUI GetFieldCardUI() {
        return cardUI;
    }

    public DomainFieldCardUI CardUI { get { return cardUI; } }
}
