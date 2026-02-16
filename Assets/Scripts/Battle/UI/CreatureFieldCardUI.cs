using TMPro;
using UnityEngine;

public class CreatureFieldCardUI : FieldCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;

    public void Init(CreatureCard card) {
        atk.text = card.GetAtk().ToString();
        health.text = card.GetHealth().ToString();
    }
}
