using UnityEngine;

[CreateAssetMenu(fileName = "Wither Status Effect Base", menuName = "Scriptable Objects/Effect/Base/Wither Status")]
public class WitherStatusEffectBase : CreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "Wither status Description"; // TODO: Update later

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new WitherStatusEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
