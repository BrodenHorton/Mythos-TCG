using UnityEngine;

[CreateAssetMenu(fileName = "Bloodthirsty Effect Base", menuName = "Scriptable Objects/Effect/Base/Bloodthirsty")]
public class BloodthirstyEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "When this creature deals damage, it gains +1/+1.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new BloodthirstyEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
