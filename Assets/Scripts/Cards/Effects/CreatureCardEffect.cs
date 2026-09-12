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

            EffectRule<StatBoostEffectContext, LifePointsChangedEventArgs> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition());
            statBoostProkRule.AddPrecondition(new LifePointsIncreasedPrecondition());
            statBoostProkRule.AddAction(new StatBoostIncrementAction());
            EffectSequence<StatBoostEffectContext, LifePointsChangedEventArgs> statBoostProkSequence = new(new LifePointsChangedTrigger());
            statBoostProkSequence.AddRule(statBoostProkRule);

            string rawDescription = GetKeywordLinkTagText("blessing", "Blessing") + ": Gain +1/+1 until the end of the turn";

            UniqueCardEffect<CreatureCard> chorionSparkletBlessing = StatBoostEffectFactory.Create(context,
                                                                                                   statBoostProkSequence,
                                                                                                   rawDescription);
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

            EffectRule<StatBoostEffectContext, PlayerEventArgs> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition());
            statBoostProkRule.AddPrecondition(new CreatureInCombatPrecondition(shouldRequireCreatureInCombat: true));
            statBoostProkRule.AddAction(new StatBoostIncrementAction());
            EffectSequence<StatBoostEffectContext, PlayerEventArgs> statBoostProkSequence = new (new DeclareAttackersStateExitedTrigger());
            statBoostProkSequence.AddRule(statBoostProkRule);

            string rawDescription = GetKeywordLinkTagText("battle_cry", "Battle Cry") + ": Gain +2/+0 until the end of the turn";

            UniqueCardEffect<CreatureCard> wildEmberbackBattleCry = StatBoostEffectFactory.Create(context,
                                                                                                   statBoostProkSequence,
                                                                                                   rawDescription);
            CardEffectRegistry.Register(CreatureCardEffectType.WildEmberbackBattleCry, wildEmberbackBattleCry);
        }
        #endregion

        #region Sinister Snail Death Cry
        {
            CardSearchEffectContext context = new CardSearchEffectContext(id: "sinister_snail_death_cry",
                                                                          effectName: "Sinister Snail Death Cry",
                                                                          searchTarget: cardRegistry.GetCardById("sinister_snail"));

            EffectRule<CardSearchEffectContext, PlayerCardEventArgs<CreatureCard>> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition());
            cardSearchRule.AddAction(new CardSearchAction());
            EffectSequence<CardSearchEffectContext, PlayerCardEventArgs<CreatureCard>> cardSearchSequence = new(new CreatureDestroyedTrigger());
            cardSearchSequence.AddRule(cardSearchRule);

            string rawDescription = GetKeywordLinkTagText("death_cry", "Death Cry") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";

            UniqueCardEffect<CreatureCard> sinisterSnailDeathCry = new UniqueCardEffect<CreatureCard>(rawDescription);
            sinisterSnailDeathCry.AddEffectSequence(cardSearchSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.SinisterSnailDeathCry, sinisterSnailDeathCry);
        }
        #endregion

        #region Squad Frog Summon
        {
            CardSearchEffectContext context = new CardSearchEffectContext(id: "squad_frog_summon",
                                                                          effectName: "Squad Frog Summon",
                                                                          searchTarget: cardRegistry.GetCardById("squad_frog"));

            EffectRule<CardSearchEffectContext, PlayerCardEventArgs<CreatureCard>> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition());
            cardSearchRule.AddAction(new CardSearchAction());
            EffectSequence<CardSearchEffectContext, PlayerCardEventArgs<CreatureCard>> cardSearchSequence = new(new SummonTrigger());
            cardSearchSequence.AddRule(cardSearchRule);

            string rawDescription = GetKeywordLinkTagText("summon", "Summon") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";

            UniqueCardEffect<CreatureCard> squadFrogSummon = new UniqueCardEffect<CreatureCard>(rawDescription);
            squadFrogSummon.AddEffectSequence(cardSearchSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.SquadFrogSummon, squadFrogSummon);
        }
        #endregion

        #region Astra's Field Medic
        {
            LifeGainEffectContext context = new LifeGainEffectContext(id: "astras_field_medic_summon",
                                                                      effectName: "Astra's Field Medic Summon",
                                                                      lifePointsModifier: 1);

            EffectRule<LifeGainEffectContext, PlayerCardEventArgs<CreatureCard>> lifeGainRule = new(context);
            lifeGainRule.AddPrecondition(new CardCheckPrecondition());
            lifeGainRule.AddAction(new LifeGainAction());
            EffectSequence<LifeGainEffectContext, PlayerCardEventArgs<CreatureCard>> lifeGainSequence = new(new SummonTrigger());
            lifeGainSequence.AddRule(lifeGainRule);

            string rawDescription = GetKeywordLinkTagText("summon", "Summon") + ": Increase life points by " + context.LifePointsModifier;

            UniqueCardEffect<CreatureCard> astrasFieldMedicSummon = new UniqueCardEffect<CreatureCard>(rawDescription);
            astrasFieldMedicSummon.AddEffectSequence(lifeGainSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.AstrasFieldMedicSummon, astrasFieldMedicSummon);
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
