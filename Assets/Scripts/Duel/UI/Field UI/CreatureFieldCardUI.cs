using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CreatureFieldCardUI : CardUI {
    [SerializeField] private TextMeshPro atk;
    [SerializeField] private TextMeshPro health;
    private bool isInCombatField;

    public void Init(ulong playerId, CreatureCardPayload card) {
        cardUuid = card.Uuid;
        this.playerId = playerId;
        isInCombatField = false;

        AddListeners();
        UpdateCardUI(card);
    }

    public override void AddListeners() {
        base.AddListeners();
        EventBus.Instance.OnCreatureEndOfTurnRegenerationFinished += UpdateCardUIOnEndOfTurnRegeneration;
        EventBus.Instance.OnCreatureTappedFinished += UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnCreatureUntappedFinished += UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnCreatureDamagedFinished += UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnCreatureHealedFinished += UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnPostCreatureDestroyed += DestroyCreature;
        EventBus.Instance.OnPostCreatureCombat += UpdateUICardOnPostCreatureCombat;
        EventBus.Instance.OnCreatureCardEffectClientpdate += UpdateCardUIOnCardPayload;
    }

    public override void RemoveListeners() {
        base.RemoveListeners();
        EventBus.Instance.OnCreatureEndOfTurnRegenerationFinished -= UpdateCardUIOnEndOfTurnRegeneration;
        EventBus.Instance.OnCreatureTappedFinished -= UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnCreatureUntappedFinished -= UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnCreatureDamagedFinished -= UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnCreatureHealedFinished -= UpdateCardUIOnPlayerCardPayload;
        EventBus.Instance.OnPostCreatureDestroyed -= DestroyCreature;
        EventBus.Instance.OnPostCreatureCombat -= UpdateUICardOnPostCreatureCombat;
        EventBus.Instance.OnCreatureCardEffectClientpdate -= UpdateCardUIOnCardPayload;
    }

    public override void SelectCard(out bool canDragCard) {
        if (!isSelectable)
            throw new System.Exception("Attempting to call SelectCard when CardUI is not marked selectable");

        canDragCard = !isInCombatField;
        EventBus.Instance.InvokeOnSelectCreatureFieldCard(new CardUIEventArgs<CreatureFieldCardUI>(this));
    }

    public override void StartCardDrag() {
        EventBus.Instance.InvokeOnStartCreatureFieldCardDrag(new CardUIEventArgs<CreatureFieldCardUI>(this));
    }

    public override void ReleaseCardDrag() {
        EventBus.Instance.InvokeOnReleaseCreatureFieldCardDrag(new CardUIEventArgs<CreatureFieldCardUI>(this));
        EventBus.Instance.InvokeOnReleaseCreatureFieldCardDragFinished(new CardUIEventArgs<CreatureFieldCardUI>(this));
    }

    public void UpdateCardUI(CreatureCardPayload card) {
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

    public void UpdateCardUIOnCardPayload(object sender, CardPayloadEventArgs<CreatureCardPayload> args) {
        if (args.CardPayload.Uuid == cardUuid)
            UpdateCardUI(args.CardPayload);
    }

    public void UpdateCardUIOnPlayerCardPayload(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (args.CardPayload.Uuid == cardUuid)
            UpdateCardUI(args.CardPayload);
    }

    public void UpdateCardUIOnEndOfTurnRegeneration(object sender, List<CreatureCardPayload> cards) {
        foreach (CreatureCardPayload card in cards) {
            if (card.Uuid == cardUuid) {
                UpdateCardUI(card);
                break;
            }
        }
    }

    public void UpdateUICardOnPostCreatureCombat(object sender, CreatureCombatPayloadEventArgs args) {
        if (args.Attacker != null && args.Attacker.Uuid == cardUuid)
            UpdateCardUI(args.Attacker);
        else if (args.Defender != null && args.Defender.Uuid == cardUuid)
            UpdateCardUI(args.Defender);
    }

    public void Tap() {
        transform.Rotate(0f, -90f, 0f, Space.World);
    }

    public void Untap() {
        transform.eulerAngles = Vector3.zero;
    }

    private void DestroyCreature(object sender, PlayerCardPayloadEventArgs<CreatureCardPayload> args) {
        if (args.CardPayload.Uuid != cardUuid)
            return;

        RemoveListeners();
        Destroy(gameObject);
    }

    public bool IsInCombatField { get { return isInCombatField; } set { isInCombatField = value; } }
}
