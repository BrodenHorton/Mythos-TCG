using System;
using Unity.Netcode;

[Serializable]
public class SwiftnessEffect : CreatureCardEffect {

    public SwiftnessEffect() : base() { }

    public SwiftnessEffect(SwiftnessEffect effect) : base() { }

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

    public override CreatureCardEffect DeepCopy() {
        return new SwiftnessEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SwiftnessEffectPayload(this);
    }
}
