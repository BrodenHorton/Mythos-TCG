using System;

[Serializable]
public class DuelistEffect : CreatureCardEffect {

    public DuelistEffect() : base() { }

    public DuelistEffect(DuelistEffect effect) : base() { }

    public override void Init(CreatureCard card) {
        this.card = card;
    }

    public override void RemoveListeners() { }

    public override CreatureCardEffect DeepCopy() {
        return new DuelistEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DuelistEffectPayload(this);
    }
}