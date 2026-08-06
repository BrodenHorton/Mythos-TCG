using UnityEngine;

[CreateAssetMenu(fileName = "Reach Effect Base", menuName = "Scriptable Objects/Effect/Base/Reach")]
public class ReachEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new ReachEffect(this);
    }
}
