using System;

[Serializable]
public class SpellshieldEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_DESCRIPTION = "The first time this card is targeted by an opponent’s effect, it is negated.";

    private SpellshieldEffectBase effectBase;

    public SpellshieldEffect(SpellshieldEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
    }

    public override void RemoveListeners() { }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SpellshieldEffectPayload(this);
    }
}
