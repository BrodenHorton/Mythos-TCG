using System;

[Serializable]
public class MenaceEffect : StaticCreatureCardEffect {
    private static readonly int BLOCKABLE_HEALTH_MIN = 4;

    private MenaceEffectBase effectBase;

    public MenaceEffect(MenaceEffectBase effectBase) {
        this.effectBase = effectBase;
    }

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

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new MenaceEffectPayload(this);
    }
}