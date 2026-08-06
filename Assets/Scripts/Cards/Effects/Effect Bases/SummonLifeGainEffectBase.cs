using UnityEngine;

[CreateAssetMenu(fileName = "Summon Life Gain Effect Base", menuName = "Scriptable Objects/Effect/Base/Summon Life Gain")]
public class SummonLifeGainEffectBase : CreatureCardEffectBase {
    [SerializeField] private int amount;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        throw new System.NotImplementedException();
        //return new BlessingStatBoostEffect(this);
    }

    public int Amount { get { return amount; } }
}