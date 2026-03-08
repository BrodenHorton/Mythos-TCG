using UnityEngine;

public class ConsoleUI : MonoBehaviour {
    [SerializeField] private GameObject logContainer;
    [SerializeField] private ConsoleInputFieldUI consoleInputField;
    [Header("Prefabs")]
    [SerializeField] private LogUI logPrefab;

    private void Awake() {
        consoleInputField.OnChatSubmission += AddChatLog;
    }

    private void Start() {
        HideInputField();
    }

    public void ShowInputField() {
        consoleInputField.gameObject.SetActive(true);
    }

    public void HideInputField() {
        consoleInputField.Clear();
        consoleInputField.gameObject.SetActive(false);
    }

    public void SubmitInputField() {
        consoleInputField.Submit();
        HideInputField();
    }

    private void AddChatLog(object sender, ChatSubmissionEventArgs args) {
        LogUI logUI = Instantiate(logPrefab);
        logUI.transform.parent = logContainer.transform;
        logUI.SetText(args.Message);
    }

    public bool IsConsoleInputFieldActive() {
        return consoleInputField.gameObject.activeSelf;
    }

    public void SetInputFieldText(string text) {
        consoleInputField.SetText(text);
    }
}
