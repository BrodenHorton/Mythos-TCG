using System;

[Serializable]
public class EnduranceEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Endurance";
    private static readonly string EFFECT_DESCRIPTION = "Attacking does not cause this creature to tap.";

    public EnduranceEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
    }

    public EnduranceEffect(EnduranceEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnCreatureTapped += CancelCreatureTap;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureTapped -= CancelCreatureTap;
    }

    private void CancelCreatureTap(object sender, PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;

        TcgLogger.Log("Endurance Effect triggered");
        args.IsCanceled = true;
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new EnduranceEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new EnduranceEffectPayload(this);
    }
}