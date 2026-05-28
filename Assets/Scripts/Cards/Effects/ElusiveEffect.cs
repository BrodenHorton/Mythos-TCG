using System;
using Unity.Netcode;

[Serializable]
public class ElusiveEffect : CreatureCardEffect {

    public ElusiveEffect() : base() { }

    public ElusiveEffect(ElusiveEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnSelectAttackerToDefend += RestrictDefenders;
        EventBus.Instance.OnSelectElusiveAttackerToDefend += SetCanDefendElusiveAttacker;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectAttackerToDefend -= RestrictDefenders;
        EventBus.Instance.OnSelectElusiveAttackerToDefend -= SetCanDefendElusiveAttacker;
    }

    private void RestrictDefenders(object sender, CanDefendEventArgs args) {
        if (args.Attacker.Uuid != creatureCardUuid)
            return;
        if (!args.CanDefend)
            return;

        TcgLogger.Log("Elusive Effect triggered");
        CanDefendEventArgs elusiveEffectArgs = new CanDefendEventArgs(args.InitiatorId,
                                                                      args.TargetId,
                                                                      args.Attacker,
                                                                      args.Defender,
                                                                      false);
        EventBus.Instance.InvokeOnSelectElusiveAttackerToDefend(elusiveEffectArgs);
        args.CanDefend = elusiveEffectArgs.CanDefend;
    }

    private void SetCanDefendElusiveAttacker(object sender, CanDefendEventArgs args) {
        if (args.Defender.Uuid != creatureCardUuid)
            return;

        TcgLogger.Log("Elusive Effect triggered");
        args.CanDefend = true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new ElusiveEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new ElusiveEffectPayload(this);
    }
}
