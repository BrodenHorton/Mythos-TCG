using System;
using TMPro;
using UnityEngine;

public class ConsoleInputField : MonoBehaviour {
    [SerializeField] private TMP_InputField inputField;

    private bool shouldUpdateText;
    private bool isActive;

    private void Start() {
        inputField.onValueChanged.AddListener((s) => {
            shouldUpdateText = true;
        });

        inputField.textComponent.textWrappingMode = TextWrappingModes.PreserveWhitespaceNoWrap;
        inputField.caretWidth = 1;
        Clear();
        inputField.ActivateInputField();
        isActive = true;
    }

    private void LateUpdate() {
        if(shouldUpdateText) {
            UpdateTextField();
            shouldUpdateText = false;
        }
    }

    private void UpdateTextField() {
        inputField.textComponent.ForceMeshUpdate();
        inputField.GetComponent<RectTransform>().sizeDelta = new Vector2(inputField.textComponent.GetPreferredValues().x, inputField.GetComponent<RectTransform>().sizeDelta.y);
    }

    public void Clear() {
        inputField.text = "";
        UpdateTextField();
    }

    public bool IsActive { get { return isActive; } set { isActive = value; } }
}
