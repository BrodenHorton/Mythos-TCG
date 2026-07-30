using System;
using UnityEngine;

[Serializable]
public class BlessingStatBoostEffect : BlessingEffect {
    [SerializeField] private bool isResetAfterTurn;
    [SerializeField] private int atkBoost;
    [SerializeField] private int healthBoost;

    private int effectProkCount;

    public BlessingStatBoostEffect() : base() {
        description = "Gain +" + atkBoost + " +" + healthBoost;
        effectProkCount = 0;
    }

    public BlessingStatBoostEffect(BlessingStatBoostEffect effect) : base() {
        description = "Gain +" + atkBoost + " +" + healthBoost;
        effectProkCount = effect.effectProkCount;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnLifePointsChanged += BlessingEffectHandler;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnLifePointsChanged -= BlessingEffectHandler;
    }

    protected override void BlessingEffectHandler(object sender, LifePointsChangedEventArgs args) {
        
    }

    public override CreatureCardEffect DeepCopy() {
        return new BlessingStatBoostEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        throw new System.NotImplementedException();
    }
}