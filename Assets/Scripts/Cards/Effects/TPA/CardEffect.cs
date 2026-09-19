using System;
using System.Collections.Generic;
using static CardRichTextUtil;

public class CardEffect<TCard> where TCard : Card {
    protected TCard card;
    protected List<IEffectSequence<TCard>> sequences;
    protected string rawDescription;

    public CardEffect(string rawDescription) {
        this.rawDescription = rawDescription;
    }

    public static void RegisterEffects() {
        CardRegistry cardRegistry = ServiceLocator.Get<CardRegistry>();

        #region Creature Card Effects

        #region Overwhelm
        {
            StaticCreatureCardEffect overwhelmEffect = OverwhelmEffectFactory.Create(id: "overwhelm",
                                                                                     effectName: "Overwhelm");
            CardEffectRegistry.Register(CreatureCardEffectType.Overwhelm, overwhelmEffect);
        }
        #endregion

        #region Bloodthirsty
        {
            StaticCreatureCardEffect bloodthirstyEffect = BloodthirstyEffectFactory.Create(id: "bloodthirsty",
                                                                                           effectName: "Bloodthirsty");
            CardEffectRegistry.Register(CreatureCardEffectType.Bloodthirsty, bloodthirstyEffect);
        }
        #endregion

        #region Swiftness
        {
            StaticCreatureCardEffect swiftnessEffect = SwiftnessEffectFactory.Create(id: "swiftness",
                                                                                     effectName: "Swiftness");
            CardEffectRegistry.Register(CreatureCardEffectType.Swiftness, swiftnessEffect);
        }
        #endregion

        #region Deathtouch
        {
            StaticCreatureCardEffect deathtouchEffect = DeathtouchEffectFactory.Create(id: "deathtouch",
                                                                                       effectName: "Deathtouch");
            CardEffectRegistry.Register(CreatureCardEffectType.Deathtouch, deathtouchEffect);
        }
        #endregion

        #region Defender
        {
            StaticCreatureCardEffect defenderEffect = DefenderEffectFactory.Create(id: "defender",
                                                                                   effectName: "Defender");
            CardEffectRegistry.Register(CreatureCardEffectType.Defender, defenderEffect);
        }
        #endregion

        #region Duelist
        {

        }
        #endregion

        #region Elusive
        {
            StaticCreatureCardEffect elusiveEffect = ElusiveEffectFactory.Create(id: "elusive",
                                                                                 effectName: "Elusive");
            CardEffectRegistry.Register(CreatureCardEffectType.Elusive, elusiveEffect);
        }
        #endregion

        #region Endurance
        {
            StaticCreatureCardEffect enduranceEffect = EnduranceEffectFactory.Create(id: "endurance",
                                                                                     effectName: "Endurance");
            CardEffectRegistry.Register(CreatureCardEffectType.Endurance, enduranceEffect);
        }
        #endregion

        #region Lifelink
        {
            StaticCreatureCardEffect lifelinkEffect = LifelinkEffectFactory.Create(id: "lifelink",
                                                                                   effectName: "Lifelink");
            CardEffectRegistry.Register(CreatureCardEffectType.Lifelink, lifelinkEffect);
        }
        #endregion

        #region Reach
        {
            StaticCreatureCardEffect reachEffect = ReachEffectFactory.Create(id: "reach",
                                                                             effectName: "Reach");
            CardEffectRegistry.Register(CreatureCardEffectType.Reach, reachEffect);
        }
        #endregion

        #region Menace
        {
            StaticCreatureCardEffect menaceEffect = MenaceEffectFactory.Create(id: "menace",
                                                                               effectName: "Menace");
            CardEffectRegistry.Register(CreatureCardEffectType.Menace, menaceEffect);
        }
        #endregion

        #region Wither
        {
            StaticCreatureCardEffect witherEffect = WitherEffectFactory.Create(id: "wither",
                                                                               effectName: "Wither");
            CardEffectRegistry.Register(CreatureCardEffectType.Wither, witherEffect);
        }
        #endregion

        #region Wither Status
        {
            CardEffect<CreatureCard> witherStatusEffect = WitherStatusEffectFactory.Create(id: "wither_status",
                                                                                           effectName: "Wither Status");
            CardEffectRegistry.Register(CreatureCardEffectType.WitherStatus, witherStatusEffect);
        }
        #endregion

        #region Chorion Sparklet Blessing
        {
            StatBoostContext context = new StatBoostContext(id: "chorion_sparklet_blessing",
                                                            effectName: "Chorion Sparklet Blessing",
                                                            atkBoost: 1,
                                                            healthBoost: 1,
                                                            isResetAfterTurn: true);

            EffectRule<StatBoostContext, LifePointsChangedEventArgs, CreatureCard> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition());
            statBoostProkRule.AddPrecondition(new LifePointsIncreasedPrecondition());
            statBoostProkRule.AddAction(new StatBoostIncrementAction());

            EffectSequence<StatBoostContext, LifePointsChangedEventArgs, CreatureCard> statBoostProkSequence = new(new LifePointsChangedTrigger());
            statBoostProkSequence.AddRule(statBoostProkRule);

            string rawDescription = GetKeywordLinkTagText("blessing", "Blessing") + ": Gain +1/+1 until the end of the turn";
            CardEffect<CreatureCard> chorionSparkletBlessing = StatBoostEffectFactory.Create(context,
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

            EffectRule<StatBoostContext, PlayerEventArgs, CreatureCard> statBoostProkRule = new(context);
            statBoostProkRule.AddPrecondition(new PlayerCheckPrecondition());
            statBoostProkRule.AddPrecondition(new CreatureInCombatPrecondition());
            statBoostProkRule.AddAction(new StatBoostIncrementAction());

            EffectSequence<StatBoostContext, PlayerEventArgs, CreatureCard> statBoostProkSequence = new(new DeclareAttackersStateExitedTrigger());
            statBoostProkSequence.AddRule(statBoostProkRule);

            string rawDescription = GetKeywordLinkTagText("battle_cry", "Battle Cry") + ": Gain +2/+0 until the end of the turn";
            CardEffect<CreatureCard> wildEmberbackBattleCry = StatBoostEffectFactory.Create(context,
                                                                                            statBoostProkSequence,
                                                                                            rawDescription);
            CardEffectRegistry.Register(CreatureCardEffectType.WildEmberbackBattleCry, wildEmberbackBattleCry);
        }
        #endregion

        #region Sinister Snail Death Cry
        {
            CardSearchContext<CreatureCard> context = new CardSearchContext<CreatureCard>(id: "sinister_snail_death_cry",
                                                                                          effectName: "Sinister Snail Death Cry",
                                                                                          searchTarget: cardRegistry.GetCardById("sinister_snail"));

            EffectRule<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
            cardSearchRule.AddAction(new CardSearchAction<CreatureCard>());

            EffectSequence<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchSequence = new(new CreatureDestroyedTrigger());
            cardSearchSequence.AddRule(cardSearchRule);

            string rawDescription = GetKeywordLinkTagText("death_cry", "Death Cry") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";
            CardEffect<CreatureCard> sinisterSnailDeathCry = new CardEffect<CreatureCard>(rawDescription);
            sinisterSnailDeathCry.AddEffectSequence(cardSearchSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.SinisterSnailDeathCry, sinisterSnailDeathCry);
        }
        #endregion

        #region Squad Frog Summon
        {
            CardSearchContext<CreatureCard> context = new CardSearchContext<CreatureCard>(id: "squad_frog_summon",
                                                                                          effectName: "Squad Frog Summon",
                                                                                          searchTarget: cardRegistry.GetCardById("squad_frog"));

            EffectRule<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchRule = new(context);
            cardSearchRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
            cardSearchRule.AddAction(new CardSearchAction<CreatureCard>());

            EffectSequence<CardSearchContext<CreatureCard>, PlayerCardEventArgs<CreatureCard>, CreatureCard> cardSearchSequence = new(new SummonTrigger());
            cardSearchSequence.AddRule(cardSearchRule);

            string rawDescription = GetKeywordLinkTagText("summon", "Summon") + ": Add a " + context.SearchTarget.CardName + " card from your deck to your hand";
            CardEffect<CreatureCard> squadFrogSummon = new CardEffect<CreatureCard>(rawDescription);
            squadFrogSummon.AddEffectSequence(cardSearchSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.SquadFrogSummon, squadFrogSummon);
        }
        #endregion

        #region Astra's Field Medic
        {
            LifeGainContext context = new LifeGainContext(id: "astras_field_medic_summon",
                                                          effectName: "Astra's Field Medic Summon",
                                                          lifePointsModifier: 1);

            EffectRule<LifeGainContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> lifeGainRule = new(context);
            lifeGainRule.AddPrecondition(new CardCheckPrecondition<CreatureCard>());
            lifeGainRule.AddAction(new LifeGainAction());

            EffectSequence<LifeGainContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> lifeGainSequence = new(new SummonTrigger());
            lifeGainSequence.AddRule(lifeGainRule);

            string rawDescription = GetKeywordLinkTagText("summon", "Summon") + ": Increase life points by " + context.LifePointsModifier;
            CardEffect<CreatureCard> astrasFieldMedicSummon = new CardEffect<CreatureCard>(rawDescription);
            astrasFieldMedicSummon.AddEffectSequence(lifeGainSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.AstrasFieldMedicSummon, astrasFieldMedicSummon);
        }
        #endregion

        #region Wirlpool Tadpole Swarm
        {
            SwarmAddEffectContext context = new SwarmAddEffectContext(id: "wirlpool_tadpole_swarm",
                                                                      effectName: "Whirlpool Tadpole Swarm",
                                                                      additionalEffectType: CreatureCardEffectType.Reach);

            EffectRule<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmAddEffectRule = new(context);
            swarmAddEffectRule.AddPrecondition(new PlayerCheckPrecondition());
            swarmAddEffectRule.AddPrecondition(new SwarmCheckPrecondition());
            swarmAddEffectRule.AddPrecondition(new SwarmAddedEffectPrecondition());
            swarmAddEffectRule.AddAction(new SwarmAddEffectAction());

            EffectSequence<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmAddEffectSequence = new(new SummonTrigger());
            swarmAddEffectSequence.AddRule(swarmAddEffectRule);

            EffectRule<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmRemoveEffectRule = new(context);
            swarmRemoveEffectRule.AddPrecondition(new PlayerCheckPrecondition());
            swarmRemoveEffectRule.AddPrecondition(new SwarmCheckPrecondition(), shouldEvaluateAsNot: true);
            swarmRemoveEffectRule.AddPrecondition(new SwarmAddedEffectPrecondition(), shouldEvaluateAsNot: true);
            swarmRemoveEffectRule.AddAction(new SwarmRemoveEffectAction());

            EffectSequence<SwarmAddEffectContext, PlayerCardEventArgs<CreatureCard>, CreatureCard> swarmRemoveEffectSequence = new(new CreatureDestroyedTrigger());
            swarmRemoveEffectSequence.AddRule(swarmAddEffectRule);

            string rawDescription = GetKeywordLinkTagText("swarm", "Swarm") + ": " + context.AdditionalEffectType;
            CardEffect<CreatureCard> wirlpoolTadpoleSwarm = new CardEffect<CreatureCard>(rawDescription);
            wirlpoolTadpoleSwarm.AddEffectSequence(swarmAddEffectSequence);
            wirlpoolTadpoleSwarm.AddEffectSequence(swarmRemoveEffectSequence);

            CardEffectRegistry.Register(CreatureCardEffectType.WirlpoolTadpoleSwarm, wirlpoolTadpoleSwarm);
        }
        #endregion

        #region Sprouting Bud Evolve
        {
            CardEffect<CreatureCard> sproutingBudEvolve = EvolveEffectFactory.Create(id: "sprouting_bud_evolve",
                                                                                     effectName: "Sprouting Bud Evolve",
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

    public string GetRawDescription() {
        return rawDescription;
    }

    public CreatureCardEffectPayload GetEffectPayload() {
        throw new NotImplementedException();
    }

    public CardEffect<TCard> Clone() {
        throw new NotImplementedException();
    }

    public TCard Card { get { return card; } }
}
