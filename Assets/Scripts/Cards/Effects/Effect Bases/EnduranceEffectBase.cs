using UnityEngine;

[CreateAssetMenu(fileName = "Endurance Effect Base", menuName = "Scriptable Objects/Effect/Base/Endurance")]
public class EnduranceEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "Attacking does not cause this creature to tap.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new EnduranceEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
