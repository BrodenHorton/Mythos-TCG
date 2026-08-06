using UnityEngine;

[CreateAssetMenu(fileName = "Defender Effect Base", menuName = "Scriptable Objects/Effect/Base/Defender")]
public class DefenderEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new DefenderEffect(this);
    }
}
