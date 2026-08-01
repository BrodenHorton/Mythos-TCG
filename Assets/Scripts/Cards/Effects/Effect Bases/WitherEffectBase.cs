using UnityEngine;

[CreateAssetMenu(fileName = "Wither Effect Base", menuName = "Scriptable Objects/Effect/Base/Wither")]
public class WitherEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "Deals damage as -1/-1 debuffs.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new WitherEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
