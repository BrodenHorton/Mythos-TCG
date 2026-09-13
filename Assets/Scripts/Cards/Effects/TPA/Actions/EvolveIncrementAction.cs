using System;

public class EvolveIncrementAction : EffectAction<EvolveContext, EventArgs> {
    private DuelManager duelManager;

    public EvolveIncrementAction() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public void Execute(EvolveContext context, EventArgs _) {
        context.CurrentTurnCount++;
        if (context.CurrentTurnCount >= context.EvolutionRequiredTurnCount) {
            MatchPlayer player = duelManager.GetPlayerById(context.Card.PlayerId);
            context.Card.DestroyCreature();
            player.PlayCard(context.Evolution.GenerateCardFromBase(player.PlayerId));
        }
    }
}
