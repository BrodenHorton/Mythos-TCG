using System;

[Serializable]
public class SpellshieldEffect : CreatureCardEffect {
    private static readonly string EFFECT_NAME = "SpellShield";
    private static readonly string EFFECT_DESCRIPTION = "The first time this card is targeted by an opponent’s effect, it is negated.";

    public SpellshieldEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
    }

    public SpellshieldEffect(SpellshieldEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
    }

    public override void RemoveListeners() { }

    public override bool IsStaticKeyword() {
        return true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new SpellshieldEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SpellshieldEffectPayload(this);
    }
}
