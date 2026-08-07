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

    protected virtual void Start() {
        CardSelectionManager.Instance.OnSetSelectableCards += SetSelectabilityOnSetSelectableCards;
    }

    protected virtual void OnDestroy() {
        FieldCardSelectionManager.Instance.OnSetSelectableFieldCards -= SetSelectabilityOnSetSelectableCards;
    }

    public abstract void SelectCard(out bool canDragCard);

    public abstract void StartCardDrag();

    public abstract void ReleaseCardDrag();

    protected void SetSelectabilityOnSetSelectableCards(object sender, List<Guid> cardUuids) {
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