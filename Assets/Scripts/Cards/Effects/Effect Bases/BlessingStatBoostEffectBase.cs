using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Blessing Effect Base", menuName = "Scriptable Objects/Effect/Base/Blessing Stat Boost")]
public class BlessingStatBoostEffectBase : BlessingEffectBase {
    [SerializeField] private int atkBoost;
    [SerializeField] private int healthBoost;
    [SerializeField] private bool isResetAfterTurn;

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new BlessingStatBoostEffect(this);
    }

    public override string GetDynamicDescription() {
        StringBuilder sb = new StringBuilder();
        sb.Append("Gain +" + atkBoost + " +" + healthBoost);
        if (isResetAfterTurn)
            sb.Append(" until the end of the turn");
        return sb.ToString();
    }

    public int AtkBoost { get { return atkBoost; } }

    public int HealthBoost { get { return healthBoost; } }

    public bool IsResetAfterTurn { get { return isResetAfterTurn; } }
}