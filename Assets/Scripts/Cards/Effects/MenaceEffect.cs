using System;
using Unity.Netcode;

[Serializable]
public class MenaceEffect : CreatureCardEffect {
    private static readonly int BLOCKABLE_HEALTH_MIN = 4;

    public MenaceEffect() : base() { }

    public MenaceEffect(MenaceEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnSelectAttackerToDefend += RestrictDefenders;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectAttackerToDefend -= RestrictDefenders;
    }

    private void RestrictDefenders(object sender, CanDefendEventArgs args) {
        if (args.Attacker.Uuid != creatureCardUuid)
            return;
        if (!args.CanDefend)
            return;

        TcgLogger.Log("Menace Effect triggered");
        if (args.Defender.GetHealth() < BLOCKABLE_HEALTH_MIN)
            args.CanDefend = false;
    }

    public override CreatureCardEffect DeepCopy() {
        return new MenaceEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new MenaceEffectPayload(this);
    }
}