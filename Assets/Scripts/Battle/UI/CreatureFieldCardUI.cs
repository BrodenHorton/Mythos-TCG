using TMPro;
using UnityEngine;

public class CreatureFieldCardUI : FieldCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;

    public void Init(CreatureCard card) {
        atk.text = card.GetAtk().ToString();
        health.text = card.GetHealth().ToString();
    }

    public void Tap() {
        transform.Rotate(0f, -90f, 0f, Space.World);
    }

    public void UnTap() {
        transform.eulerAngles = Vector3.zero;
    }
}
