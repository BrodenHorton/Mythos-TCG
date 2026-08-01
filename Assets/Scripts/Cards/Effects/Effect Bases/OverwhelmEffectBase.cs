using UnityEngine;

[CreateAssetMenu(fileName = "Overwhelm Effect Base", menuName = "Scriptable Objects/Effect/Base/Overwhelm")]
public class OverwhelmEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "Overflow damage that isn’t blocked by a defender's Health is dealt as life point damage.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new OverwhelmEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
