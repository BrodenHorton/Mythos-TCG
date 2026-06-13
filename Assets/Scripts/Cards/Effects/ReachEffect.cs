using System;
using Unity.Netcode;

[Serializable]
public class ReachEffect : CreatureCardEffect {

    public ReachEffect() : base() { }

    public ReachEffect(ReachEffect effect) : base() { }

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

    public override CreatureCardEffect DeepCopy() {
        return new ReachEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new ReachEffectPayload(this);
    }
}