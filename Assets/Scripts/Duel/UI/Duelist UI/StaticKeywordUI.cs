using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform), typeof(HorizontalLayoutGroup))]
public class StaticKeywordUI : MonoBehaviour {
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
        if(iconDatabase.Contains(effect.IconId))
            icon.sprite = iconDatabase.Get(effect.IconId);
        staticKeywordText.text = effect.EffectName.ToString().ToUpper();
        staticKeywordText.color = STATIC_KEYWORD_COLOR;
        effectDescription = effect.Description.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
