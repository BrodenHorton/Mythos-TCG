using UnityEngine;

[CreateAssetMenu(fileName = "Battle Cry Stat Boost Effect Base", menuName = "Scriptable Objects/Effect/Base/Battle Cry Stat Boost")]
public class BattleCryStatBoostEffectBase : CreatureCardEffectBase {
    [SerializeField] private int atkBoost;
    [SerializeField] private int healthBoost;
    [SerializeField] private bool isResetAfterTurn;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new BattleCryStatBoostEffect(this);
    }

    public int AtkBoost { get { return atkBoost; } }

    public int HealthBoost { get { return healthBoost; } }

    public bool IsResetAfterTurn { get { return isResetAfterTurn; } }
}
