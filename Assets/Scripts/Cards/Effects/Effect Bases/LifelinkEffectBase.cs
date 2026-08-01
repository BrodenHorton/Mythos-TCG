using UnityEngine;

[CreateAssetMenu(fileName = "Lifelink Effect Base", menuName = "Scriptable Objects/Effect/Base/Lifelink")]
public class LifelinkEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "Increase life points equal to the damage dealt to the defender.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new LifelinkEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
