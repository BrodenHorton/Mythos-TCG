using UnityEngine;
using UnityEngine.InputSystem;

public class InspectionUIController : MonoBehaviour {
    [SerializeField] private InspectionUI inspectionUI;

    private void Start() {
        CardSelectionManager.Instance.OnInspectCard += InspectCardHandler;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Escape.started += CloseInspectionUI;
    }

    private void OnDestroy() {
        CardSelectionManager.Instance.OnInspectCard -= InspectCardHandler;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Escape.started -= CloseInspectionUI;
    }

    private void InspectCardHandler(object sender, CardPayloadEventArgs<CardPayload> args) {
        InspectCard(args.CardPayload);
    }

    public void InspectCard(CardPayload cardPayload) {
        inspectionUI.InspectCard(cardPayload);
    }

    private void CloseInspectionUI(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        if (!inspectionUI.IsOpen)
            return;

        inspectionUI.Hide();
    }
}