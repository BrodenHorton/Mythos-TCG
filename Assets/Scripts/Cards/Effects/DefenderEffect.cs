using System;

[Serializable]
public class DefenderEffect : CreatureCardEffect {
    private static readonly string EFFECT_NAME = "Defender";
    private static readonly string EFFECT_DESCRIPTION = "This creature cannot declare an attack.";

    public DefenderEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
    }

    public DefenderEffect(DefenderEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnCanCreatureAttack += CancelCanAttack;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnEnteringFieldSummoningSickness -= CancelCanAttack;
    }

    private void CancelCanAttack(object sender, PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;

        TcgLogger.Log("Defender Effect triggered");
        args.IsCanceled = true;
    }

    public override bool IsStaticKeyword() {
        return true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new DefenderEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DefenderEffectPayload(this);
    }
}