using System;
using System.Text;

[Serializable]
public class BlessingStatBoostEffect : BlessingEffect {
    private BlessingStatBoostEffectBase effectBase;
    private int effectProkCount;
    private DuelStateManager stateManager;

    public BlessingStatBoostEffect(BlessingStatBoostEffectBase effectBase) {
        this.effectBase = effectBase;
        effectProkCount = 0;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        stateManager = ServiceLocator.Get<DuelStateManager>();
        EventBus.Instance.OnLifePointsChanged += BlessingEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack += AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth += AddHealth;
        stateManager.EndPhase.OnEndPhasEnteredFinished += ClearEffectProks;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnLifePointsChanged -= BlessingEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack -= AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth -= AddHealth;
        stateManager.EndPhase.OnEndPhasEnteredFinished -= ClearEffectProks;
    }

    protected override void BlessingEffectHandler(object sender, LifePointsChangedEventArgs args) {
        if (args.PlayerId != card.PlayerId)
            return;
        if (args.PreviousLifePoints >= args.LifePoints)
            return;

        TcgLogger.Log("BlessingStatBoostEffect Proked");
        effectProkCount++;
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
    }

    public override string GetEffectDescription() {
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
        return new BlessingStatBoostEffectPayload(this);
    }

    public int EffectProkCount { get { return effectProkCount; } }
}