using System;

[Serializable]
public class BloodthirstyEffect : CreatureCardEffect {
    private int effectProkCount;

    public BloodthirstyEffect() : base() { }

    public BloodthirstyEffect(BloodthirstyEffect effect) : base() {
        effectProkCount = effect.effectProkCount;
    }

    public override void Init(Guid creatureCardUuid) {
        this.creatureCardUuid = creatureCardUuid;
        EventBus.Instance.OnCreatureCombatFinished += AddEffectProk;
        EventBus.Instance.OnCalculateCreatureAttack += AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth += AddHealth;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCombatFinished -= AddEffectProk;
        EventBus.Instance.OnCalculateCreatureAttack -= AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth -= AddHealth;
    }

    private void AddEffectProk(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker == null || args.Attacker.Uuid != creatureCardUuid)
            return;

        TcgLogger.Log("Bloodthirsty Proked");
        effectProkCount++;
    }

    private void AddAttack(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != creatureCardUuid)
            return;

        args.Value += effectProkCount;
    }

    private void AddHealth(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != creatureCardUuid)
            return;

        args.Value += effectProkCount;
    }

    public override CreatureCardEffect DeepCopy() {
        return new BloodthirstyEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new BloodthirstyEffectPayload(this);
    }

    public int EffectProkCount { get { return effectProkCount; } }
}