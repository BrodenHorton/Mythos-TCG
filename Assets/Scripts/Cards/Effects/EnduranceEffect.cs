using System;
using Unity.Netcode;

[Serializable]
public class EnduranceEffect : CreatureCardEffect {

    public EnduranceEffect() : base() { }

    public EnduranceEffect(EnduranceEffect effect) : base() { }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnCreatureTapped += CancelCreatureTap;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureTapped -= CancelCreatureTap;
    }

    private void CancelCreatureTap(object sender, PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != creatureCardUuid)
            return;

        TcgLogger.Log("Endurance Effect triggered");
        args.IsCanceled = true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new EnduranceEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new EnduranceEffectPayload(this);
    }
}