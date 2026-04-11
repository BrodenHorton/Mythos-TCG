using TMPro;
using UnityEngine;

public class CreatureHandCardUI : HandCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;

    public void Init(CreatureCard card) {
        cardUuid = card.Uuid;
        cardName.text = card.CardName;
        UpdateCreatureFieldCard(card);
    }

    public void UpdateCreatureFieldCard(CreatureCard card) {
        Color manaCostColor = Color.white;
        if (card.GetManaCost() < card.BaseManaCost)
            manaCostColor = Color.green;
        else if (card.GetManaCost() > card.BaseManaCost)
            manaCostColor = Color.red;
        manaCost.color = manaCostColor;
        manaCost.text = card.GetManaCost().ToString();

        Color atkColor = Color.white;
        if (card.GetAtk() < card.BaseAtk)
            atkColor = Color.red;
        else if (card.GetAtk() > card.BaseAtk)
            atkColor = Color.green;
        atk.color = atkColor;
        atk.text = card.GetAtk().ToString();

        Color healthColor = Color.white;
        if (card.GetHealth() < card.BaseHealth)
            healthColor = Color.red;
        else if (card.GetHealth() > card.BaseHealth)
            healthColor = Color.green;
        health.color = healthColor;
        health.text = card.GetHealth().ToString();
    }
}
