using System;

public class CombatStateCheckPrecondition<TCard> : EffectPrecondition<EffectContext<TCard>, EventArgs, TCard> where TCard : Card {
    private CombatStateManager combatStateManager;

    public CombatStateCheckPrecondition() {
        combatStateManager = ServiceLocator.Get<CombatStateManager>();
    }

    public bool Evaluate(EffectContext<TCard> context, EventArgs args) {
        return combatStateManager.CurrentState == combatStateManager.DeclareAttackersState;
    }
}