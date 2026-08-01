using UnityEngine;

[CreateAssetMenu(fileName = "Defender Effect Base", menuName = "Scriptable Objects/Effect/Base/Defender")]
public class DefenderEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "This creature cannot declare an attack.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new DefenderEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
