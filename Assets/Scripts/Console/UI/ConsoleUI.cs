using UnityEngine;

public class ConsoleUI : MonoBehaviour {
    [SerializeField] private LogContainerUI logContainerUI;
    [SerializeField] private ConsoleInputFieldUI consoleInputField;
    [Header("Prefabs")]
    [SerializeField] private LogUI logPrefab;

    private bool isActive;
    private bool isOpen;

    private void Awake() {
        isActive = true;
        isOpen = false;
    }

    private void Start() {
        CloseConsole();
    }

    public void ShowConsole() {
        logContainerUI.gameObject.SetActive(true);
        isActive = true;
    }

    public void HideConsole() {
        CloseConsole();
        logContainerUI.gameObject.SetActive(false);
        isActive = false;
    }

    public void OpenConsole() {
        consoleInputField.gameObject.SetActive(true);
        logContainerUI.gameObject.SetActive(true);
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

    public void AddLog(string msg) {
        LogUI logUI = Instantiate(logPrefab);
        logUI.SetText(msg);
        logContainerUI.AddLog(logUI);
    }

    public void SetInputFieldText(string text) {
        consoleInputField.SetText(text);
    }

    public bool IsActive { get { return isActive; } }

    public bool IsOpen { get { return isOpen; } }
}
