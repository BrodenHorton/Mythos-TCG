using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class LogContainerScrollView : MonoBehaviour {
    [SerializeField] private RectTransform scrollView;
    [SerializeField] private LogContainerUI logContainerUI;
    [SerializeField] private float scrollSpeed;

    private PlayerInputActions playerInputActions;
    private RectTransform content;
    private float scrollOffset;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.ScrollUp.performed += LogContainerScrollUp;
        playerInputActions.Player.ScrollDown.performed += LogContainerScrollDown;
        playerInputActions.Enable();

        content = logContainerUI.GetComponent<RectTransform>();
        scrollOffset = 0f;

        logContainerUI.OnLogAdded += UpdateScrollOffsetOnLogAdded;
        logContainerUI.OnLogRemoved += UpdateScrollOffsetOnLogRemoved;
    }

    private void LogContainerScrollUp(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (content.sizeDelta.y <= scrollView.sizeDelta.y)
            return;
        if (content.sizeDelta.y - scrollOffset <= scrollView.sizeDelta.y)
            return;

        Debug.Log("Scrolled up");
        scrollOffset += scrollSpeed;
        if (content.sizeDelta.y - scrollOffset <= scrollView.sizeDelta.y)
            scrollOffset = content.sizeDelta.y - scrollView.sizeDelta.y;
        SetContntLocalPosition(-scrollOffset);
    }

    private void LogContainerScrollDown(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (content.sizeDelta.y <= scrollView.sizeDelta.y)
            return;
        if (scrollOffset <= 0f)
            return;

        Debug.Log("Scrolled down");
        scrollOffset = scrollOffset - scrollSpeed > 0f ? scrollOffset - scrollSpeed : 0f;
        SetContntLocalPosition(-scrollOffset);
    }

    private void UpdateScrollOffsetOnLogAdded(object sender, FloatEventArgs args) {
        Debug.Log("UpdateScrollOffsetOnAdded entered");
        float offset = args.Value;
        if (scrollOffset <= 0f) {
            scrollOffset = 0f;
            SetContntLocalPosition(-scrollOffset);
        }
        else {
            scrollOffset += offset;
            SetContntLocalPosition(-scrollOffset);
        }
    }

    private void UpdateScrollOffsetOnLogRemoved(object sender, FloatEventArgs args) {
        Debug.Log("UpdateScrollOffsetOnRemoved entered");
        float offset = args.Value;
        if (content.sizeDelta.y - scrollOffset - offset <= scrollView.sizeDelta.y) {
            Debug.Log("position updated after removal");
            scrollOffset = content.sizeDelta.y - scrollView.sizeDelta.y;
            SetContntLocalPosition(-scrollOffset);
        }
    }

    private void SetContntLocalPosition(float offset) {
        content.localPosition = new Vector3(content.localPosition.x, offset, content.localPosition.z);
    }
}