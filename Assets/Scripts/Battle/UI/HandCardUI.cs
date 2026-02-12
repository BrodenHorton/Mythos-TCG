using System;
using UnityEngine;

public class HandCardUI : MonoBehaviour {
    [SerializeField] private GameObject selectableBorder;

    private void Awake() {
        selectableBorder.SetActive(false);
    }

    public void SetBorderVisibility(bool isVisible) {
        selectableBorder.SetActive(isVisible);
    }
}
