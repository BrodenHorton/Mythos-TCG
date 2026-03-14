using System;
using TMPro;
using UnityEngine;

public class ConsoleInputFieldUI : MonoBehaviour {
    public event EventHandler<ConsoleCommandSubmissionEventArgs> OnConsoleCommandSubmission;

    [SerializeField] private TMP_InputField inputField;

    private PlayerProfile playerProfile;
    private bool shouldUpdateText;
    private bool shouldSelectInputField;

    private void Awake() {
        inputField.onDeselect.AddListener((s) => {
            inputField.ActivateInputField();
        });
        inputField.onValueChanged.AddListener((s) => {
            shouldUpdateText = true;
        });

        inputField.caretWidth = 1;
        inputField.onFocusSelectAll = false;
        inputField.textComponent.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
        Clear();
        inputField.ActivateInputField();
        shouldUpdateText = false;
    }

    private void Start() {
        playerProfile = FindFirstObjectByType<PlayerProfile>();
        if (playerProfile == null)
            throw new Exception("Unable to find object with type PlayerProfile");
    }

    private void OnEnable() {
        shouldSelectInputField = true;
    }

    private void LateUpdate() {
        if(shouldUpdateText) {
            UpdateTextField();
            shouldUpdateText = false;
        }
        if(shouldSelectInputField) {
            inputField.ActivateInputField();
            shouldSelectInputField = false;
        }
    }

    private void UpdateTextField() {
        inputField.textComponent.ForceMeshUpdate();
        inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(inputField.textComponent.GetPreferredValues().x, inputField.GetComponent<RectTransform>().sizeDelta.y);
    }

    public void Submit() {
        if (inputField.text == "")
            return;

        if (inputField.text[0] == '/' && inputField.text.Length > 1) {
            string text = inputField.text;
            text = text.Remove(0, 1);
            string[] inputArr = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (inputArr.Length == 0)
                return;
            string cmd = inputArr[0];
            string[] args = null;
            if (inputArr.Length < 2) {
                args = Array.Empty<string>();
            }
            else {
                args = new string[inputArr.Length - 1];
                for (int i = 1; i < inputArr.Length; i++)
                    args[i - 1] = inputArr[i];
            }
            OnConsoleCommandSubmission?.Invoke(this, new ConsoleCommandSubmissionEventArgs(cmd, args));
        }
        else
            TcgLogger.Log(playerProfile.GetLogPrefix() + " " + inputField.text);
    }

    public void Clear() {
        inputField.text = "";
        UpdateTextField();
    }

    public void SetText(string text) {
        Clear();
        inputField.text = text;
        inputField.caretPosition = inputField.text.Length;
        inputField.textComponent.ForceMeshUpdate();
    }
}