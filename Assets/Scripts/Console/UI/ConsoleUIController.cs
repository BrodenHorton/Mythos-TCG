using UnityEngine;
using UnityEngine.InputSystem;

public class ConsoleUIController : MonoBehaviour {
    [SerializeField] private ConsoleUI consoleUI;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Debug1.performed += ToggleConsole;
        playerInputActions.Player.OpenConsole.performed += OpenConsole;
        playerInputActions.Player.OpenConsoleCommand.performed += OpenConsoleCommand;
        playerInputActions.Player.Escape.performed += CloseConsole;
        playerInputActions.Player.Enter.performed += SubmitInputField;
    }

    private void Start() {
        TcgLogger.Instance.OnLog += AddChatLog;
    }

    private void OnDestroy() {
        playerInputActions.Player.Debug1.performed -= ToggleConsole;
        playerInputActions.Player.OpenConsole.performed -= OpenConsole;
        playerInputActions.Player.OpenConsoleCommand.performed -= OpenConsoleCommand;
        playerInputActions.Player.Escape.performed -= CloseConsole;
        playerInputActions.Player.Enter.performed -= SubmitInputField;
        playerInputActions.Disable();

        TcgLogger.Instance.OnLog -= AddChatLog;
    }

    private void ToggleConsole(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (consoleUI.IsOpen)
            return;

        if (consoleUI.IsActive)
            consoleUI.HideConsole();
        else
            consoleUI.ShowConsole();
    }

    private void OpenConsole(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!consoleUI.IsActive)
            return;
        if (consoleUI.IsOpen)
            return;

        consoleUI.OpenConsole();
    }

    private void OpenConsoleCommand(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!consoleUI.IsActive)
            return;
        if (consoleUI.IsOpen)
            return;

        consoleUI.OpenConsole();
        consoleUI.SetInputFieldText("/");
    }

    private void CloseConsole(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!consoleUI.IsActive)
            return;
        if (!consoleUI.IsOpen)
            return;

        consoleUI.CloseConsole();
    }

    private void SubmitInputField(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!consoleUI.IsActive)
            return;
        if (!consoleUI.IsOpen)
            return;

        consoleUI.SubmitInputField();
        consoleUI.CloseConsole();
    }

    public void AddChatLog(object sender, string msg) {
        consoleUI.AddLog(msg);
    }
}