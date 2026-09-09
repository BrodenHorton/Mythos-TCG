
using System;

public class CreatureInCombatPrecondition : EffectPrecondition<EffectContext<CreatureCard>, EventArgs> {
    private bool shouldRequireCreatureInCombat;
    private CombatManager combatManager;

    public CreatureInCombatPrecondition(bool shouldRequireCreatureInCombat) {
        this.shouldRequireCreatureInCombat = shouldRequireCreatureInCombat;
        combatManager = ServiceLocator.Get<CombatManager>();
    }

    public bool Evaluate(EffectContext<CreatureCard> context, EventArgs _) {
        return combatManager.IsCreatureInCombat(context.Card.Uuid) == shouldRequireCreatureInCombat;
    }
}