using System;

public class LifeGainAction : EffectAction<LifeGainEffectContext, EventArgs> {
    private DuelManager duelManager;

    public LifeGainAction() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public void Execute(LifeGainEffectContext context, EventArgs _) {
        duelManager.GetPlayerById(context.Card.PlayerId).ModifyLifePoints(context.LifePointsModifier);
    }
}
