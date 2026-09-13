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
            StatBoostContext context = new StatBoostContext(id: "chorion_sparklet_blessing",
                                                            effectName: "Chorion Sparklet Blessing",
                                                            atkBoost: 1,
                                                            healthBoost: 1,
                                                            isResetAfterTurn: true);

            EffectRule<StatBoostContext, LifePointsChangedEventArgs> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition());
            statBoostProkRule.AddPrecondition(new LifePointsIncreasedPrecondition());
            statBoostProkRule.AddAction(new StatBoostIncrementAction());

            EffectSequence<StatBoostContext, LifePointsChangedEventArgs> statBoostProkSequence = new(new LifePointsChangedTrigger());
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
            StatBoostContext context = new StatBoostContext(id: "wild_emberback_battle_cry",
                                                            effectName: "Wild Emberback Battle Cry",
                                                            atkBoost: 2,
                                                            healthBoost: 0,
                                                            isResetAfterTurn: true);

            EffectRule<StatBoostContext, PlayerEventArgs> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition());
            statBoostProkRule.AddPrecondition(new CreatureInCombatPrecondition());
            statBoostProkRule.AddAction(new StatBoostIncrementAction());

            EffectSequence<StatBoostContext, PlayerEventArgs> statBoostProkSequence = new (new DeclareAttackersStateExitedTrigger());
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
            CardSearchContext context = new CardSearchContext(id: "sinister_snail_death_cry",
                                                                          effectName: "Sinister Snail Death Cry",
                                                                          searchTarget: cardRegistry.GetCardById("sinister_snail"));

            EffectRule<CardSearchContext, PlayerCardEventArgs<CreatureCard>> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition());
            cardSearchRule.AddAction(new CardSearchAction());

            EffectSequence<CardSearchContext, PlayerCardEventArgs<CreatureCard>> cardSearchSequence = new(new CreatureDestroyedTrigger());
            cardSearchSequence.AddRule(cardSearchRule);

            string rawDescription = GetKeywordLinkTagText("death_cry", "Death Cry") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";
            UniqueCardEffect<CreatureCard> sinisterSnailDeathCry = new UniqueCardEffect<CreatureCard>(rawDescription);
            sinisterSnailDeathCry.AddEffectSequence(cardSearchSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.SinisterSnailDeathCry, sinisterSnailDeathCry);
        }
        #endregion

        #region Squad Frog Summon
        {
            CardSearchContext context = new CardSearchContext(id: "squad_frog_summon",
                                                                          effectName: "Squad Frog Summon",
                                                                          searchTarget: cardRegistry.GetCardById("squad_frog"));

            EffectRule<CardSearchContext, PlayerCardEventArgs<CreatureCard>> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition());
            cardSearchRule.AddAction(new CardSearchAction());

            EffectSequence<CardSearchContext, PlayerCardEventArgs<CreatureCard>> cardSearchSequence = new(new SummonTrigger());
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

        #region Wirlpool Tadpole Swarm
        {
            SwarmAddEffectContext context = new SwarmAddEffectContext(id: "wirlpool_tadpole_swarm",
                                                                      effectName: "Whirlpool Tadpole Swarm",
                                                                      additionalEffectType: CreatureCardEffectType.Reach);

            EffectRule<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>> swarmAddEffectRule = new(context);
            swarmAddEffectRule.AddPrecondition(new PlayerCheckPrecondition());
            swarmAddEffectRule.AddPrecondition(new SwarmCheckPrecondition());
            swarmAddEffectRule.AddPrecondition(new SwarmAddedEffectPrecondition());
            swarmAddEffectRule.AddAction(new SwarmAddEffectAction());
            
            EffectSequence<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>> swarmAddEffectSequence = new(new SummonTrigger());
            swarmAddEffectSequence.AddRule(swarmAddEffectRule);

            EffectRule<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>> swarmRemoveEffectRule = new(context);
            swarmRemoveEffectRule.AddPrecondition(new PlayerCheckPrecondition());
            swarmRemoveEffectRule.AddPrecondition(new SwarmCheckPrecondition(), shouldEvaluateAsNot: true);
            swarmRemoveEffectRule.AddPrecondition(new SwarmAddedEffectPrecondition(), shouldEvaluateAsNot: true);
            swarmRemoveEffectRule.AddAction(new SwarmRemoveEffectAction());
            
            EffectSequence<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>> swarmRemoveEffectSequence = new(new CreatureDestroyedTrigger());
            swarmRemoveEffectSequence.AddRule(swarmAddEffectRule);

            string rawDescription = GetKeywordLinkTagText("swarm", "Swarm") + ": " + context.AdditionalEffectType;
            UniqueCardEffect<CreatureCard> wirlpoolTadpoleSwarm = new UniqueCardEffect<CreatureCard>(rawDescription);
            wirlpoolTadpoleSwarm.AddEffectSequence(swarmAddEffectSequence);
            wirlpoolTadpoleSwarm.AddEffectSequence(swarmRemoveEffectSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.WirlpoolTadpoleSwarm, wirlpoolTadpoleSwarm);
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
