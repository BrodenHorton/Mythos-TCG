using UnityEngine;

[CreateAssetMenu(fileName = "Death Cry Card Search Effect Base", menuName = "Scriptable Objects/Effect/Base/Death Cry Card Search")]
public class DeathCryCardSearchEffectBase : CardSearchEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new DeathCryCardSearchEffect(this);
    }
}
