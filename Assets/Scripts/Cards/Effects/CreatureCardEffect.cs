using System;
using static CardRichTextUtil;

[Serializable]
public abstract class CreatureCardEffect {
    protected CreatureCard card;

    public CreatureCardEffect() { }

    public static void RegisterEffects() {
        CardRegistry cardRegistry = ServiceLocator.Get<CardRegistry>();

        #region Chorion Sparklet Blessing
        {
            StatBoostEffectContext context = new StatBoostEffectContext(id: "chorion_sparklet_blessing",
                                                                        effectName: "Chorion Sparklet Blessing",
                                                                        atkBoost: 1,
                                                                        healthBoost: 1,
                                                                        isResetAfterTurn: true);
            EffectRule<StatBoostEffectContext, LifePointsChangedEventArgs> rule = new EffectRule<StatBoostEffectContext, LifePointsChangedEventArgs>(context);
            rule.AddAction(new StatBoostAction());
            rule.AddPrecondition(new LifePointsIncreasedPrecondition());
            EffectSequence<StatBoostEffectContext, LifePointsChangedEventArgs> sequence = new (new LifePointsChangedTrigger());
            sequence.AddRule(rule);
            string rawDescription = GetKeywordLinkTagText("blessing", "Blessing") + ": Gain +1/+1 until the end of the turn";
            UniqueCardEffect<CreatureCard> chorionSparkletBlessing = new UniqueCardEffect<CreatureCard>(rawDescription);
            chorionSparkletBlessing.AddEffectSequence(sequence);
            CardEffectRegistry.Register(CreatureCardEffectType.ChorionSparkletBlessing, chorionSparkletBlessing);
        }
        #endregion

        #region Wild Emberback Battle Cry
        {
            StatBoostEffectContext context = new StatBoostEffectContext(id: "wild_emberback_battle_cry",
                                                                        effectName: "Wild Emberback Battle Cry",
                                                                        atkBoost: 2,
                                                                        healthBoost: 0,
                                                                        isResetAfterTurn: true);
            EffectRule<StatBoostEffectContext, PlayerEventArgs> rule = new EffectRule<StatBoostEffectContext, PlayerEventArgs>(context);
            rule.AddAction(new StatBoostAction());
            rule.AddPrecondition(new PlayerCheckPrecondition());
            rule.AddPrecondition(new CreatureInCombatPrecondition(shouldRequireCreatureInCombat: true));
            EffectSequence<StatBoostEffectContext, PlayerEventArgs> sequence = new (new DeclareAttackersStateExitedTrigger());
            sequence.AddRule(rule);
            string rawDescription = GetKeywordLinkTagText("battle_cry", "Battle Cry") + ": Gain +2/+0 until the end of the turn";
            UniqueCardEffect<CreatureCard> wildEmberbackBattleCry = new UniqueCardEffect<CreatureCard>(rawDescription);
            wildEmberbackBattleCry.AddEffectSequence(sequence);
            CardEffectRegistry.Register(CreatureCardEffectType.WildEmberbackBattleCry, wildEmberbackBattleCry);
        }
        #endregion

        #region Sinister Snail Death Cry
        {
            CardSearchEffectContext context = new CardSearchEffectContext(id: "sinister_snail_death_cry",
                                                                          effectName: "Sinister Snail Death Cry",
                                                                          searchTarget: cardRegistry.GetCardById("sinister_snail"));
            EffectRule<CardSearchEffectContext, PlayerCardEventArgs<CreatureCard>> rule = new EffectRule<CardSearchEffectContext, PlayerCardEventArgs<CreatureCard>>(context);
            rule.AddAction(new CardSearchAction());
            rule.AddPrecondition(new CardCheckPrecondition());
            EffectSequence<CardSearchEffectContext, PlayerCardEventArgs<CreatureCard>> sequence = new (new CreatureDestroyedTrigger());
            sequence.AddRule(rule);
            string rawDescription = GetKeywordLinkTagText("death_cry", "Death Cry") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";
            UniqueCardEffect<CreatureCard> sinisterSnailDeathCry = new UniqueCardEffect<CreatureCard>(rawDescription);
            sinisterSnailDeathCry.AddEffectSequence(sequence);
            CardEffectRegistry.Register(CreatureCardEffectType.WildEmberbackBattleCry, sinisterSnailDeathCry);
        }
        #endregion
    }

    public abstract void Init(CreatureCard card);

    public abstract void RemoveListeners();

    public abstract string GetRawDescription();

    public abstract CreatureCardEffectPayload GetEffectPayload();

    public abstract CreatureCardEffect Clone();

    public CreatureCard Card { get { return card; } }
}
