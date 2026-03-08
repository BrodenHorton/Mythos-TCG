using UnityEngine;
using UnityEngine.InputSystem;

public class ConsoleUIController : MonoBehaviour {
    [SerializeField] private ConsoleUI consoleUI;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.OpenConsole.performed += OpenConsole;
        playerInputActions.Player.OpenConsoleCommand.performed += OpenConsoleCommand;
        playerInputActions.Player.Escape.performed += CloseConsole;
        playerInputActions.Player.Enter.performed += SubmitInputField;
    }

    private void OpenConsole(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (consoleUI.IsConsoleInputFieldActive())
            return;

        consoleUI.ShowInputField();
    }

    private void OpenConsoleCommand(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (consoleUI.IsConsoleInputFieldActive())
            return;

        consoleUI.ShowInputField();
        consoleUI.SetInputFieldText("/");
    }

    private void CloseConsole(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!consoleUI.IsConsoleInputFieldActive())
            return;

        consoleUI.HideInputField();
    }

    private void SubmitInputField(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!consoleUI.IsConsoleInputFieldActive())
            return;

        consoleUI.SubmitInputField();
        consoleUI.HideInputField();
    }
}