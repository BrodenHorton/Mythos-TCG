using UnityEngine;

[CreateAssetMenu(fileName = "Blessing Stat Boost Effect Base", menuName = "Scriptable Objects/Effect/Base/Blessing Stat Boost")]
public class BlessingStatBoostEffectBase : CreatureCardEffectBase {
    [SerializeField] private int atkBoost;
    [SerializeField] private int healthBoost;
    [SerializeField] private bool isResetAfterTurn;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new BlessingStatBoostEffect(this);
    }

    public int AtkBoost { get { return atkBoost; } }

    public int HealthBoost { get { return healthBoost; } }

    public bool IsResetAfterTurn { get { return isResetAfterTurn; } }
}
