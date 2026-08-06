using UnityEngine;

[CreateAssetMenu(fileName = "Wither Status Effect Base", menuName = "Scriptable Objects/Effect/Base/Wither Status")]
public class WitherStatusEffectBase : CreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new WitherStatusEffect(this);
    }
}
