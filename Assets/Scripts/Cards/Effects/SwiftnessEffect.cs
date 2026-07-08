using System;

[Serializable]
public class SwiftnessEffect : CreatureCardEffect {
    private static readonly string EFFECT_NAME = "Swiftness";
    private static readonly string EFFECT_DESCRIPTION = "This creature does not have summoning sickness.";

    public SwiftnessEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
    }

    public SwiftnessEffect(SwiftnessEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnEnteringFieldSummoningSickness += RemoveSummoningSickness;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnEnteringFieldSummoningSickness -= RemoveSummoningSickness;
    }

    private void RemoveSummoningSickness(object sender, PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;

        TcgLogger.Log("Swiftness Effect triggered");
        args.IsCanceled = true;
    }

    public override bool IsStaticKeyword() {
        return true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new SwiftnessEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SwiftnessEffectPayload(this);
    }
}
