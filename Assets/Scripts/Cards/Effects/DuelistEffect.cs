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
    }

    public override void RemoveListeners() {
        FieldCardSelectionManager.Instance.OnGetSelectableFieldCards -= SetTargetCardsSelectable;
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

    // TODO: Add event handler for when a player drags a target players creature over a duelist attacker

    public override CreatureCardEffect DeepCopy() {
        return new DuelistEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DuelistEffectPayload(this);
    }
}