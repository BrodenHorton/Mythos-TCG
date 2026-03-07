using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ConsoleInputField : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI textInput;
    [SerializeField] private GameObject cursor;
    [SerializeField] private Transform cursorOrigin;

    private float cursorTimer;
    private int cursorIndex;
    private bool isActive;

    private void Start() {
        textInput.text = "";
        textInput.textWrappingMode = TextWrappingModes.PreserveWhitespace;
        Keyboard.current.onTextInput += UpdateTextInput;

        Clear();
        cursorIndex = 0;
        ResetCursorTimer();
        isActive = true;
    }

    private void Update() {
        if (!isActive)
            return;

        cursorTimer -= Time.deltaTime;

        if (cursorTimer <= 0) {
            UpdateCursor();
            cursorTimer = 0.6f;
        }

        if(Input.GetKeyDown(KeyCode.LeftArrow))
            SetCursorIndex(cursorIndex - 1);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SetCursorIndex(cursorIndex + 1);
    }

    public void UpdateTextInput(char c) {
        if (!isActive)
            return;

        if (char.IsLetterOrDigit(c) || char.IsSymbol(c) || char.IsPunctuation(c) || c == ' ') {
            if (cursorIndex == textInput.text.Length)
                textInput.text += c;
            else {
                string firstSplice = textInput.text.Substring(0, cursorIndex);
                string secondSplice = textInput.text.Substring(cursorIndex, textInput.text.Length - cursorIndex);
                firstSplice += c;
                textInput.text = firstSplice + secondSplice;
            }
            UpdateTextField();
            SetCursorIndex(cursorIndex + 1);
        }
        else if (c == '\b') {
            if(textInput.text.Length > 0 && cursorIndex != 0) {
                textInput.text = textInput.text.Remove(cursorIndex - 1, 1);
                UpdateTextField();
                SetCursorIndex(cursorIndex - 1);
            }
        }
    }

    private void UpdateTextField() {
        textInput.GetComponent<RectTransform>().sizeDelta = new Vector2(textInput.GetPreferredValues().x, textInput.GetPreferredValues().y);
        textInput.ForceMeshUpdate();
        ResetCursorTimer();
    }

    public void Clear() {
        textInput.text = "";
        UpdateTextField();
        SetCursorIndex(0);
    }

    public void SetCursorIndex(int index) {
        if (index < 0 || index > textInput.text.Length)
            return;

        ResetCursorTimer();
        cursorIndex = index;
        if (cursorIndex == 0)
            cursor.transform.position = cursorOrigin.transform.position;
        else {
            TMP_CharacterInfo charInfo = textInput.textInfo.characterInfo[cursorIndex - 1];
            cursor.transform.position = new Vector3(textInput.transform.TransformPoint(charInfo.topRight).x, cursor.transform.position.y, cursor.transform.position.z);
        }
    }

    public void UpdateCursor() {
        cursor.gameObject.SetActive(!cursor.gameObject.activeSelf);
    }

    public void ResetCursorTimer() {
        cursorTimer = 0.6f;
        cursor.gameObject.SetActive(true);
    }

    public bool IsActive {
        get { return isActive; }
        set {
            isActive = value;
            if(isActive)
                ResetCursorTimer();
        }
    }
}
