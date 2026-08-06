using UnityEngine;

[CreateAssetMenu(fileName = "Overwhelm Effect Base", menuName = "Scriptable Objects/Effect/Base/Overwhelm")]
public class OverwhelmEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new OverwhelmEffect(this);
    }
}
