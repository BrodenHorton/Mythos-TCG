using System;
using UnityEngine;

public class HandCardUI : MonoBehaviour {
    public event EventHandler<HandCardSelectedEventArgs> OnSelected;

    [SerializeField] private GameObject selectableBorder;

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
