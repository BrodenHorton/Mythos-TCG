using System;
using Unity.Netcode;

[Serializable]
public class ReachEffect : CreatureCardEffect {

    public ReachEffect() : base() { }

    public ReachEffect(ReachEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnSelectElusiveAttackerToDefend += SetCanDefendElusiveAttacker;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectElusiveAttackerToDefend -= SetCanDefendElusiveAttacker;
    }

    private void SetCanDefendElusiveAttacker(object sender, CanDefendEventArgs args) {
        if (args.Defender.Uuid != creatureCardUuid)
            return;

        TcgLogger.Log("Range Effect triggered");
        args.CanDefend = true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new ReachEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new ReachEffectPayload(this);
    }
}