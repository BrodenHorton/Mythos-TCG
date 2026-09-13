using System;

public class CreatureInCombatPrecondition : EffectPrecondition<EffectContext<CreatureCard>, EventArgs> {
    private CombatManager combatManager;

    public CreatureInCombatPrecondition() {
        combatManager = ServiceLocator.Get<CombatManager>();
    }

    public bool Evaluate(EffectContext<CreatureCard> context, EventArgs _) {
        return combatManager.IsCreatureInCombat(context.Card.Uuid);
    }
}