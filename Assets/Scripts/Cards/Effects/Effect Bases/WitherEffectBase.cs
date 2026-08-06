using UnityEngine;

[CreateAssetMenu(fileName = "Wither Effect Base", menuName = "Scriptable Objects/Effect/Base/Wither")]
public class WitherEffectBase : StaticCreatureCardEffectBase {

    [SerializeField] private WitherStatusEffectBase witherStatusEffectBase;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new WitherEffect(this);
    }

    public WitherStatusEffectBase WitherStatusEffectBase { get { return witherStatusEffectBase; } }
}
