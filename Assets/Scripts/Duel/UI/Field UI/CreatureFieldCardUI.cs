using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureFieldCardUI : FieldCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;

    public void Init(CreatureCardPayload card) {
        cardUuid = card.Uuid;

        EventBus.Instance.OnPostCreatureCombat += (sender, args) => {
            if(args.Attacker != null && args.Attacker.Uuid == cardUuid) {
                TcgLogger.Log("Attacker update entered");
                UpdateFieldCard(args.Attacker);
            }
            else if (args.Defender != null && args.Defender.Uuid == cardUuid) {
                TcgLogger.Log("Defender update entered");
                UpdateFieldCard(args.Defender);
            }
        };
        EventBus.Instance.OnCreatureEndOfTurnRegenerationFinished += UpdateFieldCardOnEndOfTurnRegeneration;

        UpdateFieldCard(card);
    }

    public void OnDestroy() {
        EventBus.Instance.OnCreatureEndOfTurnRegenerationFinished -= UpdateFieldCardOnEndOfTurnRegeneration;
    }

    public void UpdateFieldCard(CreatureCardPayload card) {
        TcgLogger.Log("CreaturefieldCard Update uuid: " + card.Uuid);
        Color atkColor = Color.white;
        if(card.Atk < card.CardBase.Atk)
            atkColor = Color.red;
        else if (card.Atk > card.CardBase.Atk)
            atkColor = Color.green;
        atk.color = atkColor;
        atk.text = card.Atk.ToString();
        TcgLogger.Log("CreaturefieldCard atk set to: " + card.Atk.ToString());

        Color healthColor = Color.white;
        if (card.Health < card.CardBase.Health)
            healthColor = Color.red;
        else if (card.Health > card.CardBase.Health)
            healthColor = Color.green;
        health.color = healthColor;
        health.text = card.Health.ToString();
        TcgLogger.Log("CreaturefieldCard health set to: " + card.Health.ToString());

        if (card.IsTapped)
            Tap();
        else
            Untap();
    }

    public void UpdateFieldCardOnEndOfTurnRegeneration(object sender, List<CreatureCardPayload> cards) {
        foreach (CreatureCardPayload card in cards) {
            if (card.Uuid == cardUuid) {
                UpdateFieldCard(card);
                break;
            }
        }
    }

    public void Tap() {
        transform.Rotate(0f, -90f, 0f, Space.World);
    }

    public void Untap() {
        transform.eulerAngles = Vector3.zero;
    }
}
