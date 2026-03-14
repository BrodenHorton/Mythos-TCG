using TMPro;
using UnityEngine;

public class LogUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private float verticalMargin;

    private RectTransform rectTransform;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string text) {
        logText.text = text;
    }

    public void UpdateLogSize(float width) {
        logText.ForceMeshUpdate();
        // Since wrapping is enabled, we have to set the width before getting preferred height.
        rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
        rectTransform.sizeDelta = new Vector2(width, logText.preferredHeight + verticalMargin);
    }
}
