using System;
using Unity.Netcode;

[Serializable]
public class SwiftnessEffect : CreatureCardEffect {

    public SwiftnessEffect() : base() { }

    public SwiftnessEffect(SwiftnessEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnSummoningSickness += RemoveSummoningSickness;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSummoningSickness -= RemoveSummoningSickness;
    }

    private void RemoveSummoningSickness(object sender, PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != creatureCardUuid)
            return;

        TcgLogger.Log("Swiftness Effect triggered");
        args.IsCanceled = true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new SwiftnessEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SwiftnessEffectPayload(this);
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) { }
}