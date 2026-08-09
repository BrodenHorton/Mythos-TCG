using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardUI : MonoBehaviour {
    [SerializeField] protected GameObject selectableBorder;

    protected Guid cardUuid;
    protected ulong playerId;
    protected bool isSelectable;

    private void Awake() {
        selectableBorder.SetActive(false);
        isSelectable = false;
    }

    public virtual void AddListeners() {
        CardSelectionManager.Instance.OnSetSelectableCards += SetSelectabilityOnSetSelectableCards;
    }

    public virtual void RemoveListeners() {
        CardSelectionManager.Instance.OnSetSelectableCards -= SetSelectabilityOnSetSelectableCards;
    }

    public abstract void SelectCard(out bool canDragCard);

    public abstract void StartCardDrag();

    public abstract void ReleaseCardDrag();

    protected void SetSelectabilityOnSetSelectableCards(object sender, List<Guid> cardUuids) {
        TcgLogger.Log("HandCardUI SetSelectability entered. " + " cardUuids Count: " + cardUuids);
        bool isSelectable = cardUuids.Contains(cardUuid);
        SetSelectable(isSelectable);
    }

    public void SetSelectable(bool isSelectable) {
        selectableBorder.SetActive(isSelectable);
        this.isSelectable = isSelectable;
    }

    public Guid CardUuid { get { return cardUuid; } }

    public ulong PlayerId { get { return playerId; } }

    public bool IsSelectable { get { return isSelectable; } }
}