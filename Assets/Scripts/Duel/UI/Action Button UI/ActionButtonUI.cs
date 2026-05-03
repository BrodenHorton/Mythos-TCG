using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionButtonUI : MonoBehaviour {
    public event EventHandler OnActionButtonPressed;

    [SerializeField] private TextMeshPro actionText;
    [SerializeField] private SpriteRenderer actionBtnCenter;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    private Camera cam;
    private bool isActive;

    private void Awake() {
        isActive = false;
        actionText.text = "";
        actionBtnCenter.color = inactiveColor;
    }

    private void Start() {
        cam = Camera.main;

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed += ButtonPressedCheck;
    }

    private void OnDestroy() {
        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.performed -= ButtonPressedCheck;
    }

    private void ButtonPressedCheck(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!isActive)
            return;
        if (this != RaycastColliderCheck())
            return;

        Execute();
    }

    private ActionButtonUI RaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        ActionButtonUI actionButtonUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<ActionButtonCollisionPointer>()) {
                actionButtonUI = hit.collider.GetComponent<ActionButtonCollisionPointer>().ActionButtonUI;
                break;
            }
        }

        return actionButtonUI;
    }

    public void Execute() {
        OnActionButtonPressed?.Invoke(this, EventArgs.Empty);
    }

    public void SetActive(string text) {
        isActive = true;
        actionText.text = text;
        actionBtnCenter.color = activeColor;
    }

    public void SetInactive(string text) {
        isActive = false;
        actionText.text = text;
        actionBtnCenter.color = inactiveColor;
    }

    public bool IsActive { get { return isActive; } }
}
