using System;

public class LifeGainAction : EffectAction<LifeGainContext, EventArgs> {
    private DuelManager duelManager;

    public LifeGainAction() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public void Execute(LifeGainContext context, EventArgs _) {
        duelManager.GetPlayerById(context.Card.PlayerId).ModifyLifePoints(context.LifePointsModifier);
    }
}
