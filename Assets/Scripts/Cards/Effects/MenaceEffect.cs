using System;

[Serializable]
public class MenaceEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Menace";
    private static readonly string EFFECT_DESCRIPTION = "This Creature cannot be blocked by creatures with 3 or less Health.";
    private static readonly int BLOCKABLE_HEALTH_MIN = 4;

    public MenaceEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
    }

    public MenaceEffect(MenaceEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnSelectAttackerToDefend += RestrictDefenders;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectAttackerToDefend -= RestrictDefenders;
    }

    private void RestrictDefenders(object sender, CanDefendEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (!args.CanDefend)
            return;

        TcgLogger.Log("Menace Effect triggered");
        if (args.Defender.GetHealth() < BLOCKABLE_HEALTH_MIN)
            args.CanDefend = false;
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new MenaceEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new MenaceEffectPayload(this);
    }
}