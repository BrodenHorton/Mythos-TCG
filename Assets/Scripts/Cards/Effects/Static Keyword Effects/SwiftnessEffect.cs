using System;

[Serializable]
public class SwiftnessEffect : StaticCreatureCardEffect {

    private SwiftnessEffectBase effectBase;

    public SwiftnessEffect(SwiftnessEffectBase effectBase) {
        this.effectBase = effectBase;
    }

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

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SwiftnessEffectPayload(this);
    }
}
