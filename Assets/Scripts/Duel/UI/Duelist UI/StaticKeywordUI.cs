using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class StaticKeywordUI : MonoBehaviour {
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI staticKeywordText;

    private string effectDescription;

    public void Init(StaticCreatureCardEffectPayload effect) {
        IconDatabase iconDatabase = ServiceLocator.Get<IconDatabase>();
        if(iconDatabase.ContainsId(effect.IconId))
            icon.sprite = iconDatabase.GetIcon(effect.IconId);
        staticKeywordText.text = effect.EffectName.ToString();
        effectDescription = effect.Description.ToString();
    }
}
