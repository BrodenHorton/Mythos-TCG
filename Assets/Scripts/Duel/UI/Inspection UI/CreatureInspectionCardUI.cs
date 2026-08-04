using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureInspectionCardUI : InspectionCardUI<CreatureCardPayload> {
    [SerializeField] private TextMeshProUGUI atk;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private RectTransform staticKeywordContainer;
    [Header("Prefabs")]
    [SerializeField] private StaticKeywordUI staticKeywordUIPrefab;

    public override void UpdateUI(CreatureCardPayload card) {
        cardName.text = card.CardBase.CardName;

        Color manaCostColor = Color.white;
        if (card.ManaCost < card.CardBase.ManaCost)
            manaCostColor = Color.green;
        else if (card.ManaCost > card.CardBase.ManaCost)
            manaCostColor = Color.red;
        manaCost.color = manaCostColor;
        manaCost.text = card.ManaCost.ToString();

        Color atkColor = Color.white;
        if (card.Atk < card.CardBase.Atk)
            atkColor = Color.red;
        else if (card.Atk > card.CardBase.Atk)
            atkColor = Color.green;
        atk.color = atkColor;
        atk.text = card.Atk.ToString();

        Color healthColor = Color.white;
        if (card.Health < card.CardBase.Health)
            healthColor = Color.red;
        else if (card.Health > card.CardBase.Health)
            healthColor = Color.green;
        health.color = healthColor;
        health.text = card.Health.ToString();

        bool hasStaticKeyword = false;
        bool hasUniqueEffect = false;
        foreach (CreatureCardEffectPayload effect in card.GetCreatureCardEffectPayloads()) {
            if (effect is StaticCreatureCardEffectPayload staticEffect) {
                hasStaticKeyword = true;
                AddStaticKeyword(staticEffect);
            }
            else {
                hasUniqueEffect = true;
                AddUniqueEffect(effect);
            }
        }
        if (!hasStaticKeyword)
            staticKeywordContainer.gameObject.SetActive(false);
        if (!hasUniqueEffect)
            uniqueEffectContainer.gameObject.SetActive(false);
    }

    private void AddStaticKeyword(StaticCreatureCardEffectPayload effect) {
        StaticKeywordUI staticKeywordUI = Instantiate(staticKeywordUIPrefab);
        staticKeywordUI.Init(effect);
        staticKeywordUI.transform.SetParent(staticKeywordContainer.transform, false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(infoContainer);
    }

    private void AddUniqueEffect(CreatureCardEffectPayload effect) {
        TextMeshProUGUI effectText = Instantiate(uniqueEffectTextPrefab, uniqueEffectContainer);
        effectText.text = effect.Description.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(effectText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(infoContainer);
    }

    public override void ClearUI() {
        foreach(Transform child in staticKeywordContainer)
            Destroy(child);
        foreach (Transform child in uniqueEffectContainer)
            Destroy(child);
    }
}
