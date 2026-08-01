using UnityEngine;

[CreateAssetMenu(fileName = "Menace Effect Base", menuName = "Scriptable Objects/Effect/Base/Menace")]
public class MenaceEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "This Creature cannot be blocked by creatures with 3 or less Health.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new MenaceEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
