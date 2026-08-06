using System;

[Serializable]
public class ReachEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_DESCRIPTION = "Can block creatures with Elusive.";

    private ReachEffectBase effectBase;

    public ReachEffect(ReachEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnSelectElusiveAttackerToDefend += SetCanDefendElusiveAttacker;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectElusiveAttackerToDefend -= SetCanDefendElusiveAttacker;
    }

    private void SetCanDefendElusiveAttacker(object sender, CanDefendEventArgs args) {
        if (args.Defender.Uuid != card.Uuid)
            return;

        TcgLogger.Log("Reach Effect triggered");
        args.CanDefend = true;
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new ReachEffectPayload(this);
    }
}