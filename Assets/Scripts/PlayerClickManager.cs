using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerClickManager : MonoBehaviour {
    private Camera cam;
    private PlayerInputActions playerInputActions;

    private void Awake() {
        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        HandCardUI cardUI = null;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if(hit.collider.GetComponent<HandCardUI>())
                cardUI = hit.collider.GetComponent<HandCardUI>();
            else if(hit.collider.GetComponentInParent<HandCardUI>())
                cardUI = hit.collider.GetComponentInParent<HandCardUI>();
        }
        if (cardUI == null)
            return;

        cardUI.Select();
    }
}
