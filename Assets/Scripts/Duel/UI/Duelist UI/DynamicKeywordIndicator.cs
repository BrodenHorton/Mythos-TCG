using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DynamicKeywordIndicator : MonoBehaviour, DynamicPopupPromptIndicator {
    private RectTransform rectTransform;
    private string keywordDescription;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(string keywordDescription) {
        this.keywordDescription = keywordDescription;
    }

    public string GetPrompt() {
        return keywordDescription;
    }

    public RectTransform RectTransform { get { return rectTransform; } }
}