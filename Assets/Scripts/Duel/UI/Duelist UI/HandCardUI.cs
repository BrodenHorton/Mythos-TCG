using System;
using TMPro;
using UnityEngine;

public abstract class HandCardUI : MonoBehaviour {
    [SerializeField] protected GameObject selectableBorder;
    [SerializeField] protected TextMeshPro cardName;
    [SerializeField] protected TextMeshPro manaCost;

    protected Guid cardUuid;

    private void Awake() {
        selectableBorder.SetActive(false);
    }

    public void SetBorderVisibility(bool isVisible) {
        selectableBorder.SetActive(isVisible);
    }

    public Guid CardUuid { get { return cardUuid; } }
}
