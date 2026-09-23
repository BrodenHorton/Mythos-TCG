using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(HorizontalLayoutGroup))]
public class StaticKeywordUI : MonoBehaviour, DynamicPopupPromptIndicator {
    private static Color32 STATIC_KEYWORD_COLOR = new Color32(250, 250, 100, 255);

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI staticKeywordText;

    private RectTransform rectTransform;
    private string effectDescription;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void Init(StaticCreatureCardEffectPayload effect) {
        IconRegistry iconDatabase = ServiceLocator.Get<IconRegistry>();
        if(iconDatabase.Contains(effect.EffectIconId))
            icon.sprite = iconDatabase.Get(effect.EffectIconId);
        staticKeywordText.text = effect.EffectName.ToString().ToUpper();
        staticKeywordText.color = STATIC_KEYWORD_COLOR;
        effectDescription = effect.RawDescription.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }

    public string GetPrompt() {
        return effectDescription;
    }
}
