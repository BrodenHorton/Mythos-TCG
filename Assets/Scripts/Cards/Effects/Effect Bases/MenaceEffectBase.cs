using UnityEngine;

[CreateAssetMenu(fileName = "Menace Effect Base", menuName = "Scriptable Objects/Effect/Base/Menace")]
public class MenaceEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new MenaceEffect(this);
    }
}
