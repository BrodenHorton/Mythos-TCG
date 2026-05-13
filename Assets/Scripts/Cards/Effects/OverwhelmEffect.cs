using System;
using Unity.Netcode;

[Serializable]
public class OverwhelmEffect : CreatureCardEffect {

    public OverwhelmEffect() : base() { }

    public OverwhelmEffect(OverwhelmEffect effect) : base() {
        effectType = CreatureCardEffectType.Overwhelm;
    }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnCreatureDamagedByCreature += DealOverwhelmDamage;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureDamagedByCreature -= DealOverwhelmDamage;
    }

    private void DealOverwhelmDamage(object sender, CreatureDamagedByCreatureEventArgs args) {
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
            args.Target.DamageLifePoints(overwhelmDamage);
    }

    public override CreatureCardEffect DeepCopy() {
        return new OverwhelmEffect(this);
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) { }
}