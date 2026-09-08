using System;
using static CardRichTextUtil;

[Serializable]
public abstract class CreatureCardEffect {
    protected CreatureCard card;

    public CreatureCardEffect() { }

    public static void RegisterEffects() {
        // Chorion Sparklet Blessing
        {
            StatBoostEffectContext context = new StatBoostEffectContext("chorion_sparklet_blessing", "Chorion Sparklet Blessing");
            EffectRule<StatBoostEffectContext, LifePointsChangedEventArgs> rule = new EffectRule<StatBoostEffectContext, LifePointsChangedEventArgs>(context);
            rule.AddAction(new StatBoostAction());
            rule.AddPrecondition(new LifePointsIncreasedPrecondition());
            EffectSequence<StatBoostEffectContext, LifePointsChangedEventArgs> sequence = new EffectSequence<StatBoostEffectContext, LifePointsChangedEventArgs>();
            sequence.SetTrigger(new LifePointsChangedTrigger());
            sequence.AddRule(rule);
            string rawDescription = GetKeywordLinkTagText("blessing", "Blessing") + ": Gain +1/+1 until the end of the turn";
            UniqueCardEffect<CreatureCard> chorionSparkletBlessing = new UniqueCardEffect<CreatureCard>(rawDescription);
            CardEffectRegistry.Register(CreatureCardEffectType.ChorionSparkletBlessing, chorionSparkletBlessing);
        }
    }

    public abstract void Init(CreatureCard card);

    public abstract void RemoveListeners();

    public abstract string GetRawDescription();

    public abstract CreatureCardEffectPayload GetEffectPayload();

    public abstract CreatureCardEffect Clone();

    public CreatureCard Card { get { return card; } }
}
