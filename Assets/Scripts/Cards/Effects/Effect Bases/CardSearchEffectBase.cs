using UnityEngine;

public abstract class CardSearchEffectBase : CreatureCardEffectBase {
    [SerializeField] private CardBase targetCard;

    public CardBase TargetCard { get { return targetCard; } }
}