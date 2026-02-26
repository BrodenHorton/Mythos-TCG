using System;
using TMPro;
using UnityEngine;

public class ActionButtonUI : MonoBehaviour {
    [SerializeField] private TextMeshPro actionText;

    private bool isActive;

    public void Execute() {
        EventBus.InvokeOnActionButtonPressed(this, EventArgs.Empty);
    }

    public void SetActionText(string text) {
        actionText.text = text;
    }
}
