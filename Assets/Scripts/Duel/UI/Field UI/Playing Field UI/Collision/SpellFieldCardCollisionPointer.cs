using System;
using UnityEngine;

public class SpellFieldCardCollisionPointer : MonoBehaviour, FieldCardCollisionPointer {
    [SerializeField] private SpellFieldCardUI cardUI;

    public Guid GetCardUuid() {
        return cardUI.CardUuid;
    }

    public ulong GetPlayerId() {
        return cardUI.PlayerId;
    }

    public FieldCardUI GetFieldCardUI() {
        return cardUI;
    }

    public SpellFieldCardUI CardUI { get { return cardUI; } }
}
