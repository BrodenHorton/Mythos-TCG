using UnityEngine;

public class FieldCardUI : MonoBehaviour
{
    [SerializeField] private GameObject selectableBorder;

    private void Awake() {
        selectableBorder.SetActive(false);
    }

    public void SetBorderVisibility(bool isVisible) {
        selectableBorder.SetActive(isVisible);
    }
}