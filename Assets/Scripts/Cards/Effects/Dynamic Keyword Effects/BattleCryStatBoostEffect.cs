using System.Text;

public class BattleCryStatBoostEffect : BattleCryEffect {
    private BattleCryStatBoostEffectBase effectBase;
    private int effectProkCount;
    private DuelStateManager stateManager;
    private CombatStateManager combatStateManager;
    private CombatManager combatManager;

    public BattleCryStatBoostEffect(BattleCryStatBoostEffectBase effectBase) {
        this.effectBase = effectBase;
        effectProkCount = 0;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        stateManager = ServiceLocator.Get<DuelStateManager>();
        combatStateManager = ServiceLocator.Get<CombatStateManager>();
        combatManager = ServiceLocator.Get<CombatManager>();

        combatStateManager.DeclareAttackersState.OnDeclareAttackersStateExited += BattleCryEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack += AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth += AddHealth;
        stateManager.EndPhase.OnEndPhasEnteredFinished += ClearEffectProks;
    }

    public override void RemoveListeners() {
        combatStateManager.DeclareAttackersState.OnDeclareAttackersStateExited -= BattleCryEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack -= AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth -= AddHealth;
        stateManager.EndPhase.OnEndPhasEnteredFinished -= ClearEffectProks;
    }

    protected override void BattleCryEffectHandler(object sender, ulong currentPlayerTurnId) {
        if (currentPlayerTurnId != card.PlayerId)
            return;
        if (!combatManager.IsCreatureInCombat(card.Uuid))
            return;
        CreatureCombat creatureCombat = combatManager.GetCreatureCombat(card.Uuid);
        if (creatureCombat.Attacker.Uuid != card.Uuid)
            return;

        TcgLogger.Log("BattleCryStatBoostEffect Proked");
        effectProkCount++;
        EventBus.Instance.InvokeOnCreatureCardEffectClientpdate(new CreatureCardPayload(card));
    }

    private void AddAttack(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;
        if (effectProkCount <= 0 || effectBase.AtkBoost <= 0)
            return;

        args.Value += effectProkCount * effectBase.AtkBoost;
    }

    private void AddHealth(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;
        if (effectProkCount <= 0 || effectBase.HealthBoost <= 0)
            return;

        args.Value += effectProkCount * effectBase.HealthBoost;
    }

    private void ClearEffectProks(object sender, ulong currentPlayerId) {
        if (card.PlayerId != currentPlayerId)
            return;
        if (!effectBase.IsResetAfterTurn)
            return;

        effectProkCount = 0;
        EventBus.Instance.InvokeOnCreatureCardEffectClientpdate(new CreatureCardPayload(card));
    }

    public override string GetDynamicEffectDescription() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Gain +" + effectBase.AtkBoost + " +" + effectBase.HealthBoost);
        if (effectBase.IsResetAfterTurn)
            sb.Append(" until the end of the turn");
        return sb.ToString();
    }

    public override CreatureCardEffectBase GetCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new BattleCryStatBoostEffectPayload(this);
    }

    public int EffectProkCount { get { return effectProkCount; } }
}
