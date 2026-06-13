using System;

[Serializable]
public class LifelinkEffect : CreatureCardEffect {
    private DuelManager duelManager;

    public LifelinkEffect() : base() { }

    public LifelinkEffect(LifelinkEffect effect) : base() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        duelManager = ServiceLocator.Get<DuelManager>();
        EventBus.Instance.OnCreatureCombatFinished += IncreaseHealth;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCombatFinished -= IncreaseHealth;
    }

    private void IncreaseHealth(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (args.Damage <= 0)
            return;

        TcgLogger.Log("Lifelink Effect triggered");
        duelManager.GetPlayerById(args.InitiatorId).ModifyLifePoints(args.Damage);
    }

    public override CreatureCardEffect DeepCopy() {
        return new LifelinkEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new LifelinkEffectPayload(this);
    }
}