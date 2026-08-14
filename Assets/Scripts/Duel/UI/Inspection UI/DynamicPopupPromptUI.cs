using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DynamicPopupPromptUI : MonoBehaviour {
    [SerializeField] private RectTransform promptParent;
    [SerializeField] private TextMeshProUGUI prompt;
    [SerializeField] private Vector2 mouseToPromptOffset;

    private bool isOpen;

    private void Awake() {
        isOpen = false;
    }

    private void Start() {
        Hide();
    }

    public void UpdateUI(DynamicPopupPromptIndicator indicator) {
        prompt.text = indicator.GetPrompt();
        LayoutRebuilder.ForceRebuildLayoutImmediate(promptParent);
        transform.position = new Vector3(Input.mousePosition.x + mouseToPromptOffset.x,
                                         Input.mousePosition.y + mouseToPromptOffset.y,
                                         transform.position.z);
        Show();
    }

    public void Show() {
        isOpen = true;
        promptParent.gameObject.SetActive(true);
    }

    public void Hide() {
        isOpen = false;
        promptParent.gameObject.SetActive(false);
    }

    public bool IsOpen { get { return isOpen; } }
}
