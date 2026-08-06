using UnityEngine;

[CreateAssetMenu(fileName = "Deathtouch Effect Base", menuName = "Scriptable Objects/Effect/Base/Deathtouch")]
public class DeathtouchEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new DeathtouchEffect(this);
    }
}
