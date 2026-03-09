using UnityEngine;

public class ConsoleUI : MonoBehaviour {
    [SerializeField] private GameObject logContainer;
    [SerializeField] private ConsoleInputFieldUI consoleInputField;
    [Header("Prefabs")]
    [SerializeField] private LogUI logPrefab;

    private bool isActive;
    private bool isOpen;

    private void Awake() {
        isActive = true;
        isOpen = false;
        consoleInputField.OnChatSubmission += AddChatLog;
    }

    private void Start() {
        CloseConsole();
    }

    public void ShowConsole() {
        logContainer.SetActive(true);
        isActive = true;
    }

    public void HideConsole() {
        CloseConsole();
        logContainer.SetActive(false);
        isActive = false;
    }

    public void OpenConsole() {
        consoleInputField.gameObject.SetActive(true);
        logContainer.SetActive(true);
        isOpen = true;
    }

    public void CloseConsole() {
        consoleInputField.Clear();
        consoleInputField.gameObject.SetActive(false);
        isOpen = false;
    }

    public void SubmitInputField() {
        consoleInputField.Submit();
        CloseConsole();
    }

    private void AddChatLog(object sender, ChatSubmissionEventArgs args) {
        LogUI logUI = Instantiate(logPrefab);
        logUI.transform.parent = logContainer.transform;
        logUI.SetText(args.Message);
    }

    public void SetInputFieldText(string text) {
        consoleInputField.SetText(text);
    }

    public bool IsActive { get { return isActive; } }

    public bool IsOpen { get { return isOpen; } }

}
