using UnityEngine;

[CreateAssetMenu(fileName = "Duelist Effect Base", menuName = "Scriptable Objects/Effect/Base/Duelist")]
public class DuelistEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "When this creature attacks, choose the enemy creature that defends.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new DuelistEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
