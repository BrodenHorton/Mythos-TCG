public class DeathtouchEffectFactory {
    public static StaticCreatureCardEffect Create() {
        EffectContext<CreatureCard> context = new();

        EffectRule<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> deathtouchRule = new(context);
        deathtouchRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        deathtouchRule.AddPrecondition(new DefenderExistsPrecondition());
        deathtouchRule.AddAction(new DestroyDefenderAction());

        EffectSequence<EffectContext<CreatureCard>, CreatureCombatDamageEventArgs, CreatureCard> deathtouchSequence = new(new CreatureDamagedByCreatureTrigger());
        deathtouchSequence.AddRule(deathtouchRule);

        string rawDescription = "When this creature deals damage to another creature, that creature dies";
        StaticCreatureCardEffect deathtouchEffect = new StaticCreatureCardEffect(effectName: "Deathtouch",
                                                                                 rawDescription,
                                                                                 effectIconId: "swords");
        deathtouchEffect.AddEffectSequence(deathtouchSequence);

        return deathtouchEffect;
    }
}
