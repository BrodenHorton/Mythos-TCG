using UnityEngine;

[CreateAssetMenu(fileName = "Reach Effect Base", menuName = "Scriptable Objects/Effect/Base/Reach")]
public class ReachEffectBase : StaticCreatureCardEffectBase {
    private static readonly string EFFECT_DESCRIPTION = "Can block creatures with Elusive.";

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new ReachEffect(this);
    }

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }
}
