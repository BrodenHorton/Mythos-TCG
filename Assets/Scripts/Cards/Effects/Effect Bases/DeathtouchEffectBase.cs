using UnityEngine;

[CreateAssetMenu(fileName = "Deathtouch Effect Base", menuName = "Scriptable Objects/Effect/Base/Deathtouch")]
public class DeathtouchEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "When this creature deals damage to another creature, that creature dies.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new DeathtouchEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
