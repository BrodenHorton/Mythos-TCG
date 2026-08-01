using UnityEngine;

[CreateAssetMenu(fileName = "Spellshield Effect Base", menuName = "Scriptable Objects/Effect/Base/Spellshield")]
public class SpellshieldEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "The first time this card is targeted by an opponent’s effect, it is negated.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new SpellshieldEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
