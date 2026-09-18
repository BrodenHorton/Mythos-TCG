public class MenaceEffectFactory {
    public static StaticCreatureCardEffect Create(string id, string effectName) {
        MenaceContext context = new(id, effectName);

        EffectRule<MenaceContext, CanDefendEventArgs, CreatureCard> menaceRule = new(context);
        menaceRule.AddPrecondition(new CreatureIsAttackerPrecondition());
        menaceRule.AddPrecondition(new CanTargetDefendPrecondition());
        menaceRule.AddAction(new MenaceAction());

        EffectSequence<MenaceContext, CanDefendEventArgs, CreatureCard> menaceSequence = new(new SelectAttackerToDefendTrigger());
        menaceSequence.AddRule(menaceRule);

        string rawDescription = "This Creature cannot be blocked by creatures with 3 or less Health";
        StaticCreatureCardEffect menaceEffect = new StaticCreatureCardEffect(rawDescription, effectIconId: "swords");
        menaceEffect.AddEffectSequence(menaceSequence);

        return menaceEffect;
    }
}