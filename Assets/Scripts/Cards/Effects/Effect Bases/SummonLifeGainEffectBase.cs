using UnityEngine;

[CreateAssetMenu(fileName = "Summon Life Gain Effect Base", menuName = "Scriptable Objects/Effect/Base/Summon Life Gain")]
public class SummonLifeGainEffectBase : CreatureCardEffectBase {
    [SerializeField] private LifeGainEffectData lifeGainEffectData;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new SummonLifeGainEffect(this);
    }

    public LifeGainEffectData LifeGainEffectData { get { return LifeGainEffectData; } }
}
