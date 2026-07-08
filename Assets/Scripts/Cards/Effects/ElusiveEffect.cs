using System;

[Serializable]
public class ElusiveEffect : CreatureCardEffect {
    private static readonly string EFFECT_NAME = "Elusive";
    private static readonly string EFFECT_DESCRIPTION = "Can only be blocked by creatures with Elusive or Reach.";

    public ElusiveEffect() : base() {
        effectName = EFFECT_NAME;
        description = EFFECT_DESCRIPTION;
    }

    public ElusiveEffect(ElusiveEffect effect) : this() { }

    public override void Init(CreatureCard card) {
        this.card = card;
        EventBus.Instance.OnSelectAttackerToDefend += RestrictDefenders;
        EventBus.Instance.OnSelectElusiveAttackerToDefend += SetCanDefendElusiveAttacker;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnSelectAttackerToDefend -= RestrictDefenders;
        EventBus.Instance.OnSelectElusiveAttackerToDefend -= SetCanDefendElusiveAttacker;
    }

    private void RestrictDefenders(object sender, CanDefendEventArgs args) {
        if (args.Attacker.Uuid != card.Uuid)
            return;
        if (!args.CanDefend)
            return;

        TcgLogger.Log("Elusive Effect triggered");
        CanDefendEventArgs elusiveEffectArgs = new CanDefendEventArgs(args.InitiatorId,
                                                                      args.TargetId,
                                                                      args.Attacker,
                                                                      args.Defender,
                                                                      false);
        EventBus.Instance.InvokeOnSelectElusiveAttackerToDefend(elusiveEffectArgs);
        args.CanDefend = elusiveEffectArgs.CanDefend;
    }

    private void SetCanDefendElusiveAttacker(object sender, CanDefendEventArgs args) {
        if (args.Defender.Uuid != card.Uuid)
            return;

        TcgLogger.Log("Elusive Effect triggered");
        args.CanDefend = true;
    }

    public override bool IsStaticKeyword() {
        return true;
    }

    public override CreatureCardEffect DeepCopy() {
        return new ElusiveEffect(this);
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new ElusiveEffectPayload(this);
    }
}
