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
            ExitHoverHand();
            previousSelection = null;
        }
        else if(handCardUI != null && playerUI.ContainsCard(handCardUI)) {
            if (previousSelection == null) {
                HoverHand(handCardUI);
                previousSelection = handCardUI;
            }
            else if(handCardUI != previousSelection) {
                ExitHoverCard(previousSelection);
                HoverCard(handCardUI);
                previousSelection = handCardUI;
            }
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
                break;
            }
        }
    }

    private HandCardUI HoverDetection() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if(hits.Length > 0)
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>())
                return hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
        }

        return null;
    }

    private void HoverHand(HandCardUI card) {
        playerUI.InspectHand();
        HoverCard(card);
    }

    private void ExitHoverHand() {
        playerUI.DefaultCardPositions();
    }

    private void HoverCard(HandCardUI card) {
        playerUI.HoverCard(card);
    }

    private void ExitHoverCard(HandCardUI card) {
        playerUI.ExitHoverCard(card);
    }
}
