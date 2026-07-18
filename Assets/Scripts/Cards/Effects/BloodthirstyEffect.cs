using System;

[Serializable]
public class BloodthirstyEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_NAME = "Bloodthirsty";
    private static readonly string EFFECT_DESCRIPTION = "When this creature deals damage, it gains +1/+1.";

    private int effectProkCount;

    public BloodthirstyEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
        effectProkCount = 0;
    }

    public BloodthirstyEffect(BloodthirstyEffect effect) : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
        effectIconId = "";
        effectProkCount = effect.effectProkCount;
    }

    public override void Init(CreatureCard card) {
        this.card = card;
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
        if (args.Attacker == null || args.Attacker.Uuid != card.Uuid)
            return;

        TcgLogger.Log("Bloodthirsty Proked");
        effectProkCount++;
    }

    private void AddAttack(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;

        args.Value += effectProkCount;
    }

    private void AddHealth(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;

        args.Value += effectProkCount;
    }

    public override string GetFullDescription() {
        return description;
    }

    public override CreatureCardEffect DeepCopy() {
        return new BloodthirstyEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new BloodthirstyEffectPayload(this);
    }

    public int EffectProkCount { get { return effectProkCount; } }
}