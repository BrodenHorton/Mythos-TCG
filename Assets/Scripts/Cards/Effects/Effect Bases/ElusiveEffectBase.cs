using UnityEngine;

[CreateAssetMenu(fileName = "Elusive Effect Base", menuName = "Scriptable Objects/Effect/Base/Elusive")]
public class ElusiveEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new ElusiveEffect(this);
    }
}
