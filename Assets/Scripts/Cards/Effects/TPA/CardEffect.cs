using System;
using System.Collections.Generic;
using static CardRichTextUtil;

public class CardEffect<TCard> where TCard : Card {
    protected string effectName;
    protected string rawDescription;
    protected TCard card;
    protected List<IEffectSequence<TCard>> sequences;

    public CardEffect(string effectName, string rawDescription) {
        this.rawDescription = rawDescription;
        this.effectName = effectName;
    }

    public static void RegisterEffects() {
        CardRegistry cardRegistry = ServiceLocator.Get<CardRegistry>();

        #region Creature Card Effects

        #region Overwhelm
        {
            StaticCreatureCardEffect overwhelmEffect = OverwhelmEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Overwhelm, overwhelmEffect);
        }
        #endregion

        #region Bloodthirsty
        {
            StaticCreatureCardEffect bloodthirstyEffect = BloodthirstyEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Bloodthirsty, bloodthirstyEffect);
        }
        #endregion

        #region Swiftness
        {
            StaticCreatureCardEffect swiftnessEffect = SwiftnessEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Swiftness, swiftnessEffect);
        }
        #endregion

        #region Deathtouch
        {
            StaticCreatureCardEffect deathtouchEffect = DeathtouchEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Deathtouch, deathtouchEffect);
        }
        #endregion

        #region Defender
        {
            StaticCreatureCardEffect defenderEffect = DefenderEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Defender, defenderEffect);
        }
        #endregion

        #region Duelist
        {
            StaticCreatureCardEffect duelistEffect = DuelistEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Duelist, duelistEffect);
        }
        #endregion

        #region Elusive
        {
            StaticCreatureCardEffect elusiveEffect = ElusiveEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Elusive, elusiveEffect);
        }
        #endregion

        #region Endurance
        {
            StaticCreatureCardEffect enduranceEffect = EnduranceEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Endurance, enduranceEffect);
        }
        #endregion

        #region Lifelink
        {
            StaticCreatureCardEffect lifelinkEffect = LifelinkEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Lifelink, lifelinkEffect);
        }
        #endregion

        #region Reach
        {
            StaticCreatureCardEffect reachEffect = ReachEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Reach, reachEffect);
        }
        #endregion

        #region Menace
        {
            StaticCreatureCardEffect menaceEffect = MenaceEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Menace, menaceEffect);
        }
        #endregion

        #region Wither
        {
            StaticCreatureCardEffect witherEffect = WitherEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.Wither, witherEffect);
        }
        #endregion

        #region Wither Status
        {
            CardEffect<CreatureCard> witherStatusEffect = WitherStatusEffectFactory.Create();
            CardEffectRegistry.Register(CreatureCardEffectType.WitherStatus, witherStatusEffect);
        }
        #endregion

        #region Chorion Sparklet Blessing
        {
            StatBoostContext context = new StatBoostContext(atkBoost: 1,
                                                            healthBoost: 1,
                                                            isResetAfterTurn: true);

            EffectRule<StatBoostContext, LifePointsChangedEventArgs, CreatureCard> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
            statBoostProkRule.AddPrecondition(new LifePointsIncreasedPrecondition());
            statBoostProkRule.AddAction(new StatBoostIncrementAction());

            EffectSequence<StatBoostContext, LifePointsChangedEventArgs, CreatureCard> statBoostProkSequence = new(new LifePointsChangedTrigger());
            statBoostProkSequence.AddRule(statBoostProkRule);

            string rawDescription = GetKeywordLinkTagText("blessing", "Blessing") + ": Gain +1/+1 until the end of the turn";
            CardEffect<CreatureCard> chorionSparkletBlessing = StatBoostEffectFactory.Create(effectName: "Chorion Sparklet Blessing",
                                                                                             rawDescription,
                                                                                             context,
                                                                                             statBoostProkSequence);
            CardEffectRegistry.Register(CreatureCardEffectType.ChorionSparkletBlessing, chorionSparkletBlessing);
        }
        #endregion

        #region Wild Emberback Battle Cry
        {
            StatBoostContext context = new StatBoostContext(atkBoost: 2,
                                                            healthBoost: 0,
                                                            isResetAfterTurn: true);

            EffectRule<StatBoostContext, PlayerEventArgs, CreatureCard> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
            statBoostProkRule.AddPrecondition(new CreatureInCombatPrecondition());
            statBoostProkRule.AddAction(new StatBoostIncrementAction());

            EffectSequence<StatBoostContext, PlayerEventArgs, CreatureCard> statBoostProkSequence = new(new DeclareAttackersStateExitedTrigger());
            statBoostProkSequence.AddRule(statBoostProkRule);

            string rawDescription = GetKeywordLinkTagText("battle_cry", "Battle Cry") + ": Gain +2/+0 until the end of the turn";
            CardEffect<CreatureCard> wildEmberbackBattleCry = StatBoostEffectFactory.Create(effectName: "Wild Emberback Battle Cry",
                                                                                            rawDescription,
                                                                                            context,
                                                                                            statBoostProkSequence);
            CardEffectRegistry.Register(CreatureCardEffectType.WildEmberbackBattleCry, wildEmberbackBattleCry);
        }
        #endregion

        #region Sinister Snail Death Cry
        {
            CardSearchContext<CreatureCard> context = new(searchTarget: cardRegistry.GetCardById("sinister_snail"));

            EffectRule<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
            cardSearchRule.AddAction(new CardSearchAction<CreatureCard>());

            EffectSequence<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchSequence = new(new CreatureDestroyedTrigger());
            cardSearchSequence.AddRule(cardSearchRule);

            string rawDescription = GetKeywordLinkTagText("death_cry", "Death Cry") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";
            CardEffect<CreatureCard> sinisterSnailDeathCry = new CardEffect<CreatureCard>(effectName: "Sinister Snail Death Cry",
                                                                                          rawDescription);
            sinisterSnailDeathCry.AddEffectSequence(cardSearchSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.SinisterSnailDeathCry, sinisterSnailDeathCry);
        }
        #endregion

        #region Squad Frog Summon
        {
            CardSearchContext<CreatureCard> context = new(searchTarget: cardRegistry.GetCardById("squad_frog"));

            EffectRule<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
            cardSearchRule.AddAction(new CardSearchAction<CreatureCard>());

            EffectSequence<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchSequence = new(new SummonTrigger());
            cardSearchSequence.AddRule(cardSearchRule);

            string rawDescription = GetKeywordLinkTagText("summon", "Summon") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";
            CardEffect<CreatureCard> squadFrogSummon = new CardEffect<CreatureCard>(effectName: "Squad Frog Summon",
                                                                                    rawDescription);
            squadFrogSummon.AddEffectSequence(cardSearchSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.SquadFrogSummon, squadFrogSummon);
        }
        #endregion

        #region Astra's Field Medic Summon
        {
            LifeGainContext context = new(lifePointsModifier: 1);

            EffectRule<LifeGainContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> lifeGainRule = new(context);
            lifeGainRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
            lifeGainRule.AddAction(new LifeGainAction());

            EffectSequence<LifeGainContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> lifeGainSequence = new(new SummonTrigger());
            lifeGainSequence.AddRule(lifeGainRule);

            string rawDescription = GetKeywordLinkTagText("summon", "Summon") + ": Increase life points by " + context.LifePointsModifier;
            CardEffect<CreatureCard> astrasFieldMedicSummon = new CardEffect<CreatureCard>(effectName: "Astra's Field Medic Summon",
                                                                                           rawDescription);
            astrasFieldMedicSummon.AddEffectSequence(lifeGainSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.AstrasFieldMedicSummon, astrasFieldMedicSummon);
        }
        #endregion

        #region Wirlpool Tadpole Swarm
        {
            SwarmAddEffectContext context = new(additionalEffectType: CreatureCardEffectType.Reach);

            EffectRule<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmAddEffectRule = new(context);
            swarmAddEffectRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
            swarmAddEffectRule.AddPrecondition(new SwarmCheckPrecondition());
            swarmAddEffectRule.AddPrecondition(new SwarmAddedEffectPrecondition());
            swarmAddEffectRule.AddAction(new SwarmAddEffectAction());

            EffectSequence<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmAddEffectSequence = new(new SummonTrigger());
            swarmAddEffectSequence.AddRule(swarmAddEffectRule);

            EffectRule<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmRemoveEffectRule = new(context);
            swarmRemoveEffectRule.AddPrecondition(new PlayerCheckPrecondition<CreatureCard>());
            swarmRemoveEffectRule.AddPrecondition(new SwarmCheckPrecondition(), shouldEvaluateAsNot: true);
            swarmRemoveEffectRule.AddPrecondition(new SwarmAddedEffectPrecondition(), shouldEvaluateAsNot: true);
            swarmRemoveEffectRule.AddAction(new SwarmRemoveEffectAction());

            EffectSequence<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmRemoveEffectSequence = new(new CreatureDestroyedTrigger());
            swarmRemoveEffectSequence.AddRule(swarmAddEffectRule);

            string rawDescription = GetKeywordLinkTagText("swarm", "Swarm") + ": " + context.AdditionalEffectType;
            CardEffect<CreatureCard> wirlpoolTadpoleSwarm = new CardEffect<CreatureCard>(effectName: "Whirlpool Tadpole Swarm",
                                                                                         rawDescription);
            wirlpoolTadpoleSwarm.AddEffectSequence(swarmAddEffectSequence);
            wirlpoolTadpoleSwarm.AddEffectSequence(swarmRemoveEffectSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.WirlpoolTadpoleSwarm, wirlpoolTadpoleSwarm);
        }
        #endregion

        #region Sprouting Bud Evolve
        {
            CardEffect<CreatureCard> sproutingBudEvolve = EvolveEffectFactory.Create(effectName: "Sprouting Bud Evolve",
                                                                                     evolution: cardRegistry.GetCreatureCardById("ent_warrior"),
                                                                                     evolutionRequiredTurnCount: 1);
            CardEffectRegistry.Register(CreatureCardEffectType.SproutingBudEvolve, sproutingBudEvolve);
        }
        #endregion

        #endregion
    }

    public void Init(TCard card) {
        for(int i = 0; i < sequences.Count; i++)
            sequences[i].Init(card);
    }

    public void RemoveListeners() {
        for (int i = 0; i < sequences.Count; i++)
            sequences[i].RemoveListeners();
    }

    public void AddEffectSequence(IEffectSequence<TCard> sequence) {
        sequences.Add(sequence);
    }

    public CreatureCardEffectPayload GetEffectPayload() {
        throw new NotImplementedException();
    }

    public CardEffect<TCard> Clone() {
        throw new NotImplementedException();
    }

    public string EffectName { get { return effectName; } }

    public string RawDescription { get { return rawDescription; } }
    
    public TCard Card { get { return card; } }
}
