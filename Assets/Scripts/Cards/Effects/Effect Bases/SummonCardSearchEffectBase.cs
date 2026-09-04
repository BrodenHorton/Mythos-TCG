using UnityEngine;

[CreateAssetMenu(fileName = "Summon Card Search Effect Base", menuName = "Scriptable Objects/Effect/Base/Summon Card Search")]
public class SummonCardSearchEffectBase : CreatureCardEffectBase {
    [SerializeField] private CardBase targetCard;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new SummonCardSearchEffect(this);
    }

    public CardBase TargetCard { get { return targetCard; } }
}