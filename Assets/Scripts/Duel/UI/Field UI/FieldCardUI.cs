using System;
using System.Collections.Generic;
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

    private void Start() {
        FieldCardSelectionManager.Instance.OnSetSelectableFieldCards += SetSelectabilityOnSetSelectableFieldCards;
        EventBus.Instance.OnPostCreatureDestroyed += DestroyFieldCardUI;
    }

    private void OnDestroy() {
        FieldCardSelectionManager.Instance.OnSetSelectableFieldCards -= SetSelectabilityOnSetSelectableFieldCards;
        EventBus.Instance.OnPostCreatureDestroyed -= DestroyFieldCardUI;
    }

    private void SetSelectabilityOnSetSelectableFieldCards(object sender, List<Guid> fieldCardUuids) {
        bool isSelectable = fieldCardUuids.Contains(cardUuid);
        SetSelectable(isSelectable);
    }

    public void SetSelectable(bool isSelectable) {
        selectableBorder.SetActive(isSelectable);
        this.isSelectable = isSelectable;
    }

    private void DestroyFieldCardUI(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (args.CardPayload.Uuid != cardUuid)
            return;

        FieldCardSelectionManager.Instance.OnSetSelectableFieldCards -= SetSelectabilityOnSetSelectableFieldCards;
        EventBus.Instance.OnPostCreatureDestroyed -= DestroyFieldCardUI;
        Destroy(gameObject);
    }

    public Guid CardUuid { get { return cardUuid; } }

    public ulong PlayerId { get { return playerId; } }

    public bool IsSelectable { get { return isSelectable; } }
}
