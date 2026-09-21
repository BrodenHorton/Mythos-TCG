using System;

public class CreatureInCombatWithDefenderPrecondition : EffectPrecondition<EffectContext<CreatureCard>, EventArgs, CreatureCard> {
    private CombatManager combatManager;

    public CreatureInCombatWithDefenderPrecondition() {
        combatManager = ServiceLocator.Get<CombatManager>();
    }

    public bool Evaluate(EffectContext<CreatureCard> context, EventArgs args) {
        CreatureCombat creatureCombat = combatManager.GetCreatureCombat(context.Card.Uuid);
        return creatureCombat.Defender != null;
    }
}
