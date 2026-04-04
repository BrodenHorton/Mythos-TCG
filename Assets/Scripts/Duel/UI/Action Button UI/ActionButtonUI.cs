using System;
using TMPro;
using UnityEngine;

public class ActionButtonUI : MonoBehaviour {
    [SerializeField] private TextMeshPro actionText;
    [SerializeField] private SpriteRenderer actionBtnCenter;
    [SerializeField] private Color activeColor;
    [SerializeField] private Color inactiveColor;

    private bool isActive;

    private void Awake() {
        isActive = false;
        actionText.text = "";
        actionBtnCenter.color = inactiveColor;
    }

    public void Execute() {
        EventBus.InvokeOnActionButtonPressed(this, EventArgs.Empty);
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
