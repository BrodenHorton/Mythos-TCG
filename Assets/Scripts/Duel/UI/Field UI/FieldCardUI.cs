using System;
using System.Collections.Generic;

public abstract class FieldCardUI : CardUI {

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

    private void DestroyFieldCardUI(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (args.CardPayload.Uuid != cardUuid)
            return;

        FieldCardSelectionManager.Instance.OnSetSelectableFieldCards -= SetSelectabilityOnSetSelectableFieldCards;
        EventBus.Instance.OnPostCreatureDestroyed -= DestroyFieldCardUI;
        Destroy(gameObject);
    }
}
