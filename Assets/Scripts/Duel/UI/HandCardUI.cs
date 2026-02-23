using System;
using TMPro;
using UnityEngine;

public abstract class HandCardUI : MonoBehaviour {
    public event EventHandler<HandCardSelectedEventArgs> OnSelected;

    [SerializeField] protected GameObject selectableBorder;
    [SerializeField] protected TextMeshPro manaCost;

    private void Awake() {
        selectableBorder.SetActive(false);
    }

    public void SetBorderVisibility(bool isVisible) {
        selectableBorder.SetActive(isVisible);
    }

    public void Select() {
        OnSelected?.Invoke(this, new HandCardSelectedEventArgs(this));
    }
}
