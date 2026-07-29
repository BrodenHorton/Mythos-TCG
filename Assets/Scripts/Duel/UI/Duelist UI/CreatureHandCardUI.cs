using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatureHandCardUI : HandCardUI {
    [SerializeField] private TextMeshProUGUI atk;
    [SerializeField] private TextMeshProUGUI health;
    [SerializeField] private RectTransform infoContainer;
    [SerializeField] private RectTransform staticKeywordContainer;
    [SerializeField] private RectTransform uniqueEffectContainer;
    [Header("Prefabs")]
    [SerializeField] private StaticKeywordUI staticKeywordUIPrefab;

    public void Init(CreatureCardPayload card) {
        cardUuid = Guid.Parse(card.Uuid.ToString());
        cardName.text = card.CardBase.CardName;
        bool hasStaticKeyword = false;
        bool hasUniqueEffect = false;
        foreach (CreatureCardEffectPayload effect in card.GetCreatureCardEffectPayloads()) {
            if (effect is StaticCreatureCardEffectPayload staticEffect) {
                hasStaticKeyword = true;
                AddStaticKeyword(staticEffect);
            }
            else {
                hasUniqueEffect = true;
                // TODO: Add unique effect
            }
        }
        if (!hasStaticKeyword)
            staticKeywordContainer.gameObject.SetActive(false);
        if(!hasUniqueEffect)
            uniqueEffectContainer.gameObject.SetActive(false);
        UpdateCreatureFieldCard(card);
    }

    public void UpdateCreatureFieldCard(CreatureCardPayload card) {
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
    }

    private void AddStaticKeyword(StaticCreatureCardEffectPayload effect) {
        StaticKeywordUI staticKeywordUI = Instantiate(staticKeywordUIPrefab);
        staticKeywordUI.Init(effect);
        staticKeywordUI.transform.SetParent(staticKeywordContainer.transform, false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(infoContainer);
    }
}
