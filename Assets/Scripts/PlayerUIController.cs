using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour {
    [SerializeField] private PlayerUI playerUI;
    
    private Camera cam;
    private PlayerInputActions playerInputActions;
    private HandCardUI previousSelection;

    private void Awake() {
        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
        previousSelection = null;
    }

    private void Update() {
        HandCardUI handCardUI = HoverDetection();
        if(handCardUI == null && previousSelection != null) {
            DeselectHand();
            previousSelection = null;
        }
        if (handCardUI != null && previousSelection == null) {
            InspectHand();
            previousSelection = handCardUI;
        }
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach(RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>()) {
                HandCardUI cardUI = hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
                cardUI.Select();
            }
        }
    }

    private HandCardUI HoverDetection() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>()) {
                return hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
            }
        }

        return null;
    }

    private void InspectHand() {
        playerUI.InspectHand();
    }

    private void DeselectHand() {
        playerUI.DefaultCardPositions();
    }
}
