using UnityEngine;

[CreateAssetMenu(fileName = "Bloodthirsty Effect Base", menuName = "Scriptable Objects/Effect/Base/Bloodthirsty")]
public class BloodthirstyEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new BloodthirstyEffect(this);
    }
}
