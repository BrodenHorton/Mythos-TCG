using UnityEngine;

[CreateAssetMenu(fileName = "Battle Cry Stat Boost Effect Base", menuName = "Scriptable Objects/Effect/Base/Battle Cry Stat Boost")]
public class BattleCryStatBoostEffectBase : CreatureCardEffectBase {
    [SerializeField] private StatBoostEffectData statBoostEffectData;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new BattleCryStatBoostEffect(this);
    }

    public StatBoostEffectData StatBoosEffectData { get { return statBoostEffectData; } }
}
