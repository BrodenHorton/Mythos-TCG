using System;
using System.Text;

[Serializable]
public class BlessingStatBoostEffect : BlessingEffect {
    private int atkBoost;
    private int healthBoost;
    private bool isResetAfterTurn;
    private int effectProkCount;

    public BlessingStatBoostEffect() : base() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Gain +" + atkBoost + " +" + healthBoost);
        if (isResetAfterTurn)
            sb.Append(" until the end of the turn");
        description = sb.ToString();
        effectName = dynamicEffectName;
        effectProkCount = 0;
    }

    public BlessingStatBoostEffect(BlessingStatBoostEffect effect) : base() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Gain +" + atkBoost + " +" + healthBoost);
        if (isResetAfterTurn)
            sb.Append(" until the end of the turn");
        description = sb.ToString();
        effectName = dynamicEffectName;
        effectProkCount = effect.effectProkCount;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnLifePointsChanged += BlessingEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack += AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth += AddHealth;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnLifePointsChanged -= BlessingEffectHandler;
        EventBus.Instance.OnCalculateCreatureAttack -= AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth -= AddHealth;
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
        if (effectProkCount <= 0 || atkBoost <= 0)
            return;

        args.Value += effectProkCount * atkBoost;
    }

    private void AddHealth(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;
        if (effectProkCount <= 0 || healthBoost <= 0)
            return;

        args.Value += effectProkCount * healthBoost;
    }

    public override CreatureCardEffect DeepCopy() {
        return new BlessingStatBoostEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        throw new System.NotImplementedException();
    }
}