using UnityEngine;
using UnityEngine.InputSystem;

public class LogContainerScrollView : MonoBehaviour {
    [SerializeField] private RectTransform scrollView;
    [SerializeField] private RectTransform content;
    [SerializeField] private float scrollSpeed;

    private PlayerInputActions playerInputActions;
    private float scrollOffset;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.ScrollUp.performed += LogContainerScrollUp;
        playerInputActions.Player.ScrollDown.performed += LogContainerScrollDown;
        playerInputActions.Enable();

        scrollOffset = 0f;
    }

    private void LogContainerScrollUp(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (content.sizeDelta.y <= scrollView.sizeDelta.y)
            return;
        if (content.sizeDelta.y - scrollOffset <= scrollView.sizeDelta.y)
            return;

        scrollOffset += scrollSpeed;
        if (content.sizeDelta.y - scrollOffset <= scrollView.sizeDelta.y)
            scrollOffset = content.sizeDelta.y - scrollView.sizeDelta.y;
        content.localPosition = new Vector3(0f, -scrollOffset, 0f);
    }

    private void LogContainerScrollDown(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (content.sizeDelta.y <= scrollView.sizeDelta.y)
            return;
        if (scrollOffset == 0f)
            return;

        scrollOffset = scrollOffset - scrollSpeed > 0f ? scrollOffset - scrollSpeed : 0f;
        content.localPosition = new Vector3(0f, -scrollOffset, 0f);
    }
}