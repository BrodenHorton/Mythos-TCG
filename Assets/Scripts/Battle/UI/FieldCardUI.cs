using System;
using UnityEngine;

public abstract class FieldCardUI : MonoBehaviour {
    [SerializeField] protected GameObject selectableBorder;

    protected Guid cardUuid;

    private void Awake() {
        selectableBorder.SetActive(false);
    }

    public void SetBorderVisibility(bool isVisible) {
        selectableBorder.SetActive(isVisible);
    }

    public Guid CardUuid { get { return cardUuid; } }
}
