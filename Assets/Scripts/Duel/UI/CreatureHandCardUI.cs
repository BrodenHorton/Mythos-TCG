using TMPro;
using UnityEngine;

public class CreatureHandCardUI : HandCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;

    public void Init(CreatureCard card) {
        atk.text = card.GetAtk().ToString();
        health.text = card.GetHealth().ToString();
        manaCost.text = card.GetManaCost().ToString();
    }
}
