using System;

[Serializable]
public class DuelistEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Duelist";
    private static readonly string EFFECT_DESCRIPTION = "When this creature attacks, choose the enemy creature that defends.";

    private DuelManager duelManager;
    private CombatStateManager combatStateManager;
    private CombatManager combatManager;
    private CreatureCard duelistDefender;

    public DuelistEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
        duelistDefender = null;
    }

    public DuelistEffect(DuelistEffect effect) : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
        duelistDefender = effect.duelistDefender;
    }

    public override void Init(CreatureCard card) {
        this.card = card;

        duelManager = ServiceLocator.Get<DuelManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        combatManager = ServiceLocator.Get<CombatManager>();

        FieldCardSelectionManager.Instance.OnGetSelectableFieldCards += SetTargetCardsSelectable;
        FieldCardSelectionManager.Instance.OnCreatureReleasedOverCreature += SetDuelistDefender;
        FieldCardSelectionManager.Instance.OnGetSelectableFieldCards += RemoveDuelistTargetFromSelectableCards;
        EventBus.Instance.OnUndeclareAttacker += ClearDuelistDefender;
    }

    public override void RemoveListeners() {
        FieldCardSelectionManager.Instance.OnGetSelectableFieldCards -= SetTargetCardsSelectable;
        FieldCardSelectionManager.Instance.OnCreatureReleasedOverCreature -= SetDuelistDefender;
        FieldCardSelectionManager.Instance.OnGetSelectableFieldCards -= RemoveDuelistTargetFromSelectableCards;
        EventBus.Instance.OnUndeclareAttacker -= ClearDuelistDefender;
    }

    private void SetTargetCardsSelectable(object sender, SelectableCardsEventArgs args) {
        if (args.PlayerId != card.PlayerId)
            return;
        if (duelManager.GetCurrentPlayerTurn().PlayerId != card.PlayerId)
            return;
        if (combatStateManager.CurrentState != combatStateManager.DeclareAttackersState)
            return;
        if (!combatManager.IsCreatureInCombat(card.Uuid))
            return;
        CreatureCombat creatureCombat = combatManager.GetCreatureCombat(card.Uuid);
        if (creatureCombat.Defender != null)
            return;

        DuelistCombat duelistCombat = combatManager.GetDuelistCombat(card.Uuid);
        MatchPlayer target = duelManager.GetPlayerById(duelistCombat.TargetId);
        foreach(CreatureCard creature in target.Creatures) {
            if(!combatManager.IsCreatureInCombat(creature.Uuid))
                args.CardUuids.Add(creature.Uuid);
        }
    }

    private void SetDuelistDefender(object sender, CreatureReleasedOverCreatureEventArgs args) {
        if (args.DraggingPlayerId != card.PlayerId)
            return;
        if (args.HoveredCard.Uuid != card.Uuid)
            return;
        if (duelManager.GetCurrentPlayerTurn().PlayerId != card.PlayerId)
            return;
        if (combatStateManager.CurrentState != combatStateManager.DeclareAttackersState)
            return;
        if (!combatManager.HasExistingDuelistCombat(card.PlayerId, args.HeldCard.PlayerId))
            return;
        if (!combatManager.IsCreatureInCombat(card.Uuid))
            return;
        if (combatManager.IsCreatureInCombat(args.HeldCard.Uuid))
            return;
        CreatureCombat creatureCombat = combatManager.GetCreatureCombat(card.Uuid);
        if (creatureCombat.Defender != null)
            return;

        TcgLogger.Log("Duelist Set Targets Activated");
        duelistDefender = args.HeldCard;
        combatManager.DeclareDefender(args.HeldCard.PlayerId, args.HoveredCard, args.HeldCard);
    }

    private void RemoveDuelistTargetFromSelectableCards(object sender, SelectableCardsEventArgs args) {
        if (duelistDefender == null)
            return;
        if (args.PlayerId != duelistDefender.PlayerId)
            return;
        if (!args.CardUuids.Contains(duelistDefender.Uuid))
            return;

        TcgLogger.Log("Duelist Remove Defender from Selectable Cards Activated");
        args.CardUuids.Remove(duelistDefender.Uuid);
    }

    private void ClearDuelistDefender(object sender, CombatCreatureEventArgs args) {
        if (args.InitiatorId != card.PlayerId)
            return;
        if (duelistDefender == null)
            return;
        if (args.TargetId != duelistDefender.PlayerId)
            return;

        duelistDefender = null;
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new DuelistEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DuelistEffectPayload(this);
    }
}