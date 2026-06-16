using System;
using UnityEngine;

public abstract class FieldCardUI : MonoBehaviour {
    [SerializeField] protected GameObject selectableBorder;

    protected Guid cardUuid;
    protected ulong playerId;
    protected bool isSelectable;

    private void Awake() {
        selectableBorder.SetActive(false);
        isSelectable = false;
    }

    public void SetSelectable(bool isSelectable) {
        selectableBorder.SetActive(isSelectable);
        this.isSelectable = isSelectable;
    }

    public Guid CardUuid { get { return cardUuid; } }

    public ulong PlayerId { get { return playerId; } }

    public bool IsSelectable { get { return isSelectable; } }
}
