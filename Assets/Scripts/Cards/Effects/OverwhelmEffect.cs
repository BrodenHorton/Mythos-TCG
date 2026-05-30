using System;
using Unity.Netcode;

[Serializable]
public class OverwhelmEffect : CreatureCardEffect {
    private DuelManager duelManager;

    public OverwhelmEffect() : base() { }

    public OverwhelmEffect(OverwhelmEffect effect) : base() {
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnCreatureDamagedByCreature += DealOverwhelmDamage;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDamagedByCreature -= DealOverwhelmDamage;
    }

    private void DealOverwhelmDamage(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != creatureCardUuid)
            return;
        if (args.IsCanceled)
            return;

        TcgLogger.Log("Overwhelm Effect triggered");
        int overwhelmDamage = 0;
        if (args.Defender == null)
            overwhelmDamage = args.Damage;
        else if (args.Defender.GetHealth() < args.Damage)
            overwhelmDamage = args.Damage - args.Defender.GetHealth();

        if (overwhelmDamage > 0)
            args.DirectDamage = overwhelmDamage;
    }

    public override CreatureCardEffect DeepCopy() {
        return new OverwhelmEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new OverwhelmEffectPayload(this);
    }
}
