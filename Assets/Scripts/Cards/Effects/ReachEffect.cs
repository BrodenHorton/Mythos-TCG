using System;

[Serializable]
public class ReachEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Reach";
    private static readonly string EFFECT_DESCRIPTION = "Can block creatures with Elusive.";

    public ReachEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
    }

    public ReachEffect(ReachEffect effect) : this() { }

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

        TcgLogger.Log("Range Effect triggered");
        args.CanDefend = true;
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new ReachEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new ReachEffectPayload(this);
    }
}