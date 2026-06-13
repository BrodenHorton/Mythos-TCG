using System;

[Serializable]
public class DeathtouchEffect : CreatureCardEffect {

    public DeathtouchEffect() : base() { }

    public DeathtouchEffect(DeathtouchEffect effect) : base() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnCreatureCombatFinished += DestroyDefender;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCreatureCombatFinished -= DestroyDefender;
    }

    private void DestroyDefender(object sender, CreatureCombatDamageEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (args.Defender == null)
            return;

        TcgLogger.Log("Deathtouch Effect triggered");
        args.Defender.DestroyCreature();
    }

    public override CreatureCardEffect DeepCopy() {
        return new DeathtouchEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new DeathtouchEffectPayload(this);
    }
}