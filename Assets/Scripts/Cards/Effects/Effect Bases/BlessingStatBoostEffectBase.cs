using UnityEngine;

[CreateAssetMenu(fileName = "Blessing Stat Boost Effect Base", menuName = "Scriptable Objects/Effect/Base/Blessing Stat Boost")]
public class BlessingStatBoostEffectBase : CreatureCardEffectBase {
    [SerializeField] private StatBoostEffectData statBoostEffectData;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new BlessingStatBoostEffect(this);
    }

    public StatBoostEffectData StatBoosEffectData { get { return statBoostEffectData; } }
}
