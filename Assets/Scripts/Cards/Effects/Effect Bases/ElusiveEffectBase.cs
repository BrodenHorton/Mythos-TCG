using UnityEngine;

[CreateAssetMenu(fileName = "Elusive Effect Base", menuName = "Scriptable Objects/Effect/Base/Elusive")]
public class ElusiveEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "Can only be blocked by creatures with Elusive or Reach.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new ElusiveEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
