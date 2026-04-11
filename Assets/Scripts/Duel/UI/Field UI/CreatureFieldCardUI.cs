using TMPro;
using UnityEngine;

public class CreatureFieldCardUI : FieldCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;

    public void Init(CreatureCard card) {
        cardUuid = card.Uuid;
        UpdateCreatureFieldCard(card);
    }

    public void UpdateCreatureFieldCard(CreatureCard card) {
        Color atkColor = Color.white;
        if(card.GetAtk() < card.BaseAtk)
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

    public void Tap() {
        transform.Rotate(0f, -90f, 0f, Space.World);
    }

    public void Untap() {
        transform.eulerAngles = Vector3.zero;
    }
}
