using System;

[Serializable]
public class SpellshieldEffect : CreatureCardEffect {

    public SpellshieldEffect() : base() { }

    public SpellshieldEffect(SpellshieldEffect effect) : base() { }

    public override void Init(CreatureCard card) {
        this.card = card;
    }

    public override void RemoveListeners() { }

    public override CreatureCardEffect DeepCopy() {
        return new SpellshieldEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SpellshieldEffectPayload(this);
    }
}
