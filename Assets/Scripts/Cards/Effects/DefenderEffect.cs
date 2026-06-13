using System;
using Unity.Netcode;

[Serializable]
public class DefenderEffect : CreatureCardEffect {

    public DefenderEffect() : base() { }

    public DefenderEffect(DefenderEffect effect) : base() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnCanCreatureAttack += CancelCanAttack;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnEnteringFieldSummoningSickness -= CancelCanAttack;
    }

    private void CancelCanAttack(object sender, PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
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
}