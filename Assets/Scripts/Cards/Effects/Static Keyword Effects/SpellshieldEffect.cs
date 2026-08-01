using System;

[Serializable]
public class SpellshieldEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "SpellShield";

    private SpellshieldEffectBase effectBase;

    public SpellshieldEffect(SpellshieldEffectBase effectBase) {
        this.effectBase = effectBase;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
    }

    public override void RemoveListeners() { }

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new SpellshieldEffectPayload(this);
    }
}
