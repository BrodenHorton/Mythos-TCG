using UnityEngine;

[CreateAssetMenu(fileName = "Endurance Effect Base", menuName = "Scriptable Objects/Effect/Base/Endurance")]
public class EnduranceEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new EnduranceEffect(this);
    }
}
