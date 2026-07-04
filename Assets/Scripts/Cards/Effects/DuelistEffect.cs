using System;

[Serializable]
public class DuelistEffect : CreatureCardEffect {
    private DuelManager duelManager;
    private CombatStateManager combatStateManager;
    private CombatManager combatManager;

    public DuelistEffect() : base() { }

    public DuelistEffect(DuelistEffect effect) : base() {
        duelManager = ServiceLocator.Get<DuelManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        combatManager = ServiceLocator.Get<CombatManager>();
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        FieldCardSelectionManager.Instance.OnGetSelectableFieldCards += SetTargetCardsSelectable;
        FieldCardSelectionManager.Instance.OnCreatureReleasedOverCreature += SetDuelistDefender;
    }

    public override void RemoveListeners() {
        FieldCardSelectionManager.Instance.OnGetSelectableFieldCards -= SetTargetCardsSelectable;
        FieldCardSelectionManager.Instance.OnCreatureReleasedOverCreature -= SetDuelistDefender;
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
        if (combatManager.IsCreatureInCombat(card.Uuid))
            return;
        if (combatManager.IsCreatureInCombat(args.HeldCard.Uuid))
            return;
        CreatureCombat creatureCombat = combatManager.GetCreatureCombat(card.Uuid);
        if (creatureCombat.Defender != null)
            return;

        creatureCombat.Defender = args.HeldCard;
    }

    public override CreatureCardEffect DeepCopy() {
        return new DuelistEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DuelistEffectPayload(this);
    }
}