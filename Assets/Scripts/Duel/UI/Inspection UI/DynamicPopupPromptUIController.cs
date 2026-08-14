using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicPopupPromptUIController : MonoBehaviour {
    [SerializeField] private DynamicPopupPromptUI popupPromptUI;
    [SerializeField] private EventSystem eventSystem;

    private void Update() {
        if (!GameInputManager.Instance.PlayerInputActions.UI.enabled) {
            if (popupPromptUI.IsOpen)
                popupPromptUI.Hide();

            return;
        }
        if (!IndicatorRaycast(out DynamicPopupPromptIndicator indicator)) {
            if (popupPromptUI.IsOpen)
                popupPromptUI.Hide();

            return;
        }

        popupPromptUI.UpdateUI(indicator);
    }

    private bool IndicatorRaycast(out DynamicPopupPromptIndicator indicator) {
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        eventSystem.RaycastAll(pointerData, results);

        indicator = null;
        foreach (RaycastResult entry in results) {
            if(entry.gameObject.TryGetComponent(out DynamicPopupPromptIndicator promptIndicator)) {
                indicator = promptIndicator;
                return true;
            }
        }

        return false;
    }
}
