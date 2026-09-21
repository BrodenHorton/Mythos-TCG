public class PlayerTurnCheckPrecondition<TCard> : EffectPrecondition<EffectContext<TCard>, PlayerEventArgs, TCard> where TCard : Card {
    private DuelManager duelManager;

    public PlayerTurnCheckPrecondition() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }
    
    public bool Evaluate(EffectContext<TCard> context, PlayerEventArgs args) {
        return duelManager.GetCurrentPlayerTurn().PlayerId == args.PlayerId;
    }
}