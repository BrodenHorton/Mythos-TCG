using System;

[Serializable]
public class DefenderEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_DESCRIPTION = "This creature cannot declare an attack.";

    private DefenderEffectBase effectBase;

    public DefenderEffect(DefenderEffectBase effectBase) {
        this.effectBase = effectBase;
    }

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

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DefenderEffectPayload(this);
    }
}