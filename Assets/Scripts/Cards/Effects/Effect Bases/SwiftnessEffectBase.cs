using UnityEngine;

[CreateAssetMenu(fileName = "Swiftness Effect Base", menuName = "Scriptable Objects/Effect/Base/Swiftness")]
public class SwiftnessEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new SwiftnessEffect(this);
    }
}
