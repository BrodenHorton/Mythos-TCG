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

        #region Sword Saint Miel Overwhelm
        {
            StaticCreatureCardEffect swordSaintMielOverwhelm = OverwhelmEffectFactory.Create(id: "sword_saint_miel_overwhelm",
                                                                                             effectName: "Sword Saint Miel Overwhelm");
            CardEffectRegistry.Register(CreatureCardEffectType.SwordSaintMielOverwhelm, swordSaintMielOverwhelm);
        }
        #endregion

        #region Fluttersky Bloodthirsty
        {
            StaticCreatureCardEffect flutterSkyBloodthirsty = BloodthirstyEffectFactory.Create(id: "fluttersky_bloodthirsty",
                                                                                               effectName: "Fluttersky Bloodthirsty");
            CardEffectRegistry.Register(CreatureCardEffectType.FlutterskyBloodthirsty, flutterSkyBloodthirsty);
        }
        #endregion

        #region Kaitros, Deep Sea Devourer Deathtouch
        {
            StaticCreatureCardEffect kaitrosDeepSeaDevourerDeathtouch = DeathtouchEffectFactory.Create(id: "kaitros_deep_sea_devourer_deathtouch",
                                                                                             effectName: "Kaitros, Deep Sea Devourer Deathtouch");
            CardEffectRegistry.Register(CreatureCardEffectType.KaitrosDeepSeaDevourerDeathtouch, kaitrosDeepSeaDevourerDeathtouch);
        }
        #endregion

        #region Mineral Crab Defender
        {
            StaticCreatureCardEffect mineralCrabDefender = DefenderEffectFactory.Create(id: "mineral_crab_defender",
                                                                                        effectName: "Mineral Crab Defender");
            CardEffectRegistry.Register(CreatureCardEffectType.MineralCrabDefender, mineralCrabDefender);
        }
        #endregion

        #region Whimsical Bee Elusive
        {
            StaticCreatureCardEffect whimsicalBeeElusive = ElusiveEffectFactory.Create(id: "whimsical_bee_elusive",
                                                                                       effectName: "Whimsical Bee Elusive");
            CardEffectRegistry.Register(CreatureCardEffectType.WhimsicalBeeElusive, whimsicalBeeElusive);
        }
        #endregion

        #region Sunlit Militia Endurance
        {
            StaticCreatureCardEffect sunlitMilitiaEndurance = EnduranceEffectFactory.Create(id: "sunlit_militia_endurance",
                                                                                            effectName: "Sunlit Militia Endurance");
            CardEffectRegistry.Register(CreatureCardEffectType.SunlitMilitiaEndurance, sunlitMilitiaEndurance);
        }
        #endregion

        #region Glimmirsap Frog Lifelink
        {
            StaticCreatureCardEffect glimmirsapFrogLifelink = LifelinkEffectFactory.Create(id: "glimmirsap_frog_lifelink",
                                                                                           effectName: "Glimmirsap Frog Lifelink");
            CardEffectRegistry.Register(CreatureCardEffectType.GlimmirsapFrogLifelink, glimmirsapFrogLifelink);
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
