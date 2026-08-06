using UnityEngine;

[CreateAssetMenu(fileName = "Summon Life Gain Effect Base", menuName = "Scriptable Objects/Effect/Base/Summon Life Gain")]
public class SummonLifeGainEffectBase : CreatureCardEffectBase {
    [SerializeField] private int lifePointsModifier;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new SummonLifeGainEffect(this);
    }

    public int LifePointsModifier { get { return lifePointsModifier; } }
}