using System;

[Serializable]
public class ElusiveEffect : StaticCreatureCardEffect {
    private static readonly string EFFECT_DESCRIPTION = "Can only be blocked by creatures with Elusive or Reach.";

    private ElusiveEffectBase effectBase;

    public ElusiveEffect(ElusiveEffectBase effectBase) {
        this.effectBase = effectBase;
    }

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

    public override string GetFullDescription() {
        return EFFECT_DESCRIPTION;
    }

    public override StaticCreatureCardEffectBase GetStaticCreatureEffectBase() {
        return effectBase;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        return new ElusiveEffectPayload(this);
    }
}
