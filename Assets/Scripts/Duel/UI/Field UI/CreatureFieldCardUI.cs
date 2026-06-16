using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureFieldCardUI : FieldCardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;
    private bool isInCombatField;

    public void Init(ulong playerId, CreatureCardPayload card) {
        cardUuid = card.Uuid;
        this.playerId = playerId;
        isInCombatField = false;

        EventBus.Instance.OnCreatureEndOfTurnRegenerationFinished += UpdateFieldCardOnEndOfTurnRegeneration;
        EventBus.Instance.OnCreatureTappedFinished += UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnCreatureUntappedFinished += UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnCreatureDamagedFinished += UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnCreatureHealedFinished += UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnPostCreatureCombat += UpdateFieldCardOnPostCreatureCombat;

        UpdateFieldCard(card);
    }

    public void OnDestroy() {
        EventBus.Instance.OnCreatureEndOfTurnRegenerationFinished -= UpdateFieldCardOnEndOfTurnRegeneration;
        EventBus.Instance.OnCreatureTappedFinished -= UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnCreatureUntappedFinished -= UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnCreatureDamagedFinished -= UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnCreatureHealedFinished -= UpdateFieldCardOnPlayerFieldCardPayload;
        EventBus.Instance.OnPostCreatureCombat -= UpdateFieldCardOnPostCreatureCombat;
    }

    public void UpdateFieldCard(CreatureCardPayload card) {
        Color atkColor = Color.white;
        if(card.Atk < card.CardBase.Atk)
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

        if (card.IsTapped)
            Tap();
        else
            Untap();
    }

    public void UpdateFieldCardOnPlayerFieldCardPayload(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (args.CardPayload.Uuid == cardUuid)
            UpdateFieldCard(args.CardPayload);
    }

    public void UpdateFieldCardOnEndOfTurnRegeneration(object sender, List<CreatureCardPayload> cards) {
        foreach (CreatureCardPayload card in cards) {
            if (card.Uuid == cardUuid) {
                UpdateFieldCard(card);
                break;
            }
        }
    }

    public void UpdateFieldCardOnPostCreatureCombat(object sender, CreatureCombatPayloadEventArgs args) {
        if (args.Attacker != null && args.Attacker.Uuid == cardUuid)
            UpdateFieldCard(args.Attacker);
        else if (args.Defender != null && args.Defender.Uuid == cardUuid)
            UpdateFieldCard(args.Defender);
    }

    public void Tap() {
        transform.Rotate(0f, -90f, 0f, Space.World);
    }

    public void Untap() {
        transform.eulerAngles = Vector3.zero;
    }

    public bool IsInCombatField { get { return isInCombatField; } set { isInCombatField = value; } }
}
