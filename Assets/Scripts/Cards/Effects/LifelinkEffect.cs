using System;

[Serializable]
public class LifelinkEffect : CreatureCardEffect {
    private static readonly string EFFECT_NAME = "Life Link";
    private static readonly string EFFECT_DESCRIPTION = "Increase life points equal to the damage dealt to the defender.";

    private DuelManager duelManager;

    public LifelinkEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
    }

    public LifelinkEffect(LifelinkEffect effect) : this() { }

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

    public override bool IsStaticKeyword() {
        return true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new LifelinkEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new LifelinkEffectPayload(this);
    }
}
