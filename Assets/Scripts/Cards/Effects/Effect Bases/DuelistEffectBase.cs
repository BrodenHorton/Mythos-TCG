using UnityEngine;

[CreateAssetMenu(fileName = "Duelist Effect Base", menuName = "Scriptable Objects/Effect/Base/Duelist")]
public class DuelistEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new DuelistEffect(this);
    }
}
