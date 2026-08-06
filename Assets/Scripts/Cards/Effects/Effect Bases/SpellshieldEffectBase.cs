using UnityEngine;

[CreateAssetMenu(fileName = "Spellshield Effect Base", menuName = "Scriptable Objects/Effect/Base/Spellshield")]
public class SpellshieldEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new SpellshieldEffect(this);
    }
}
