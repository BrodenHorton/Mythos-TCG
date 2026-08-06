using UnityEngine;

[CreateAssetMenu(fileName = "Lifelink Effect Base", menuName = "Scriptable Objects/Effect/Base/Lifelink")]
public class LifelinkEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new LifelinkEffect(this);
    }
}
