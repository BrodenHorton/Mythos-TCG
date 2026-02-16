using UnityEngine;

public abstract class FieldCardUI : MonoBehaviour {
    [SerializeField] protected GameObject selectableBorder;

    private void Awake() {
        selectableBorder.SetActive(false);
    }

    public void SetBorderVisibility(bool isVisible) {
        selectableBorder.SetActive(isVisible);
    }
}
