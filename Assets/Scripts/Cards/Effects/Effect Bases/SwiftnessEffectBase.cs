using UnityEngine;

[CreateAssetMenu(fileName = "Swiftness Effect Base", menuName = "Scriptable Objects/Effect/Base/Swiftness")]
public class SwiftnessEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "This creature does not have summoning sickness.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new SwiftnessEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
