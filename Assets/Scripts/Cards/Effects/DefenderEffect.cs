using System;
using Unity.Netcode;

[Serializable]
public class DefenderEffect : CreatureCardEffect {

    public DefenderEffect() : base() { }

    public DefenderEffect(DefenderEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnCanCreatureAttack += CancelCanAttack;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnEnteringFieldSummoningSickness -= CancelCanAttack;
    }

    private void CancelCanAttack(object sender, PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != creatureCardUuid)
            return;

        TcgLogger.Log("Defender Effect triggered");
        args.IsCanceled = true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new DefenderEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DefenderEffectPayload(this);
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) { }
}