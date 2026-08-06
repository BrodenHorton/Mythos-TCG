using System;

[Serializable]
public class EnduranceEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_DESCRIPTION = "Attacking does not cause this creature to tap.";

    private EnduranceEffectBase effectBase;

    public EnduranceEffect(EnduranceEffectBase effectBase) {
        this.effectBase = effectBase;
    }

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
        return EFFECT_DESCRIPTION;
    }

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new EnduranceEffectPayload(this);
    }
}