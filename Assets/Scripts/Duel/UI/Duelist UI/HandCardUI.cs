using TMPro;
using UnityEngine;

public abstract class HandCardUI : CardUI {
    [SerializeField] protected TextMeshProUGUI cardName;
    [SerializeField] protected TextMeshProUGUI manaCost;
    [SerializeField] protected RectTransform infoContainer;
    [SerializeField] protected RectTransform uniqueEffectContainer;
    [SerializeField] protected TextMeshProUGUI uniqueEffectTextPrefab;

    public sealed override void SelectCard(out bool canDragCard) {
        if (!isSelectable)
            throw new System.Exception("Attempting to call SelectCard a CardUI that is not marked as selectable");

        canDragCard = true;
    }

    public sealed override void StartCardDrag() {
        EventBus.Instance.InvokeOnStartHandCardDrag(new CardUIEventArgs<HandCardUI>(this));
    }

    public sealed override void ReleaseCardDrag() {
        EventBus.Instance.InvokeOnReleaseHandCardDrag(new CardUIEventArgs<HandCardUI>(this));
    }
}
