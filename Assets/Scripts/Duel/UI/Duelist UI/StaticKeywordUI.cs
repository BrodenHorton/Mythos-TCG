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
        IconDatabase iconDatabase = ServiceLocator.Get<IconDatabase>();
        if(iconDatabase.ContainsId(effect.IconId))
            icon.sprite = iconDatabase.GetIcon(effect.IconId);
        staticKeywordText.text = effect.EffectName.ToString().ToUpper();
        staticKeywordText.color = STATIC_KEYWORD_COLOR;
        effectDescription = effect.Description.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
