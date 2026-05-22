using System;
using TMPro;
using UnityEngine;

public class CreatureHandCardUI : HandCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;

    public void Init(CreatureCardPayload card) {
        cardUuid = Guid.Parse(card.Uuid.ToString());
        cardName.text = card.CardBase.CardName;
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
}
