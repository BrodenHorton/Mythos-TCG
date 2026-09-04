using UnityEngine;

[CreateAssetMenu(fileName = "Death Cry Card Search Effect Base", menuName = "Scriptable Objects/Effect/Base/Death Cry Card Search")]
public class DeathCryCardSearchEffectBase : CreatureCardEffectBase {
    [SerializeField] private CardBase targetCard;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new DeathCryCardSearchEffect(this);
    }

    public CardBase TargetCard { get { return targetCard; } }
}
