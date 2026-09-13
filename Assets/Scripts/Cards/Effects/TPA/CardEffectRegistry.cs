using System;
using System.Collections.Generic;

public static class CardEffectRegistry {
    private static Dictionary<CreatureCardEffectType, CardEffect<CreatureCard>> creatureCardEffectByType = new();

    public static void Register(CreatureCardEffectType type, CardEffect<CreatureCard> effect) {
        if (creatureCardEffectByType.ContainsKey(type))
            throw new Exception("Attempting to add a Creature Card Effect Type that already exists in the registry: " + type);

        creatureCardEffectByType.Add(type, effect);
    }

    public static CardEffect<CreatureCard> Get(CreatureCardEffectType type) {
        if (!creatureCardEffectByType.ContainsKey(type))
            throw new Exception("Unable to find the following Creature Card Effect Type: " + type);

        return creatureCardEffectByType[type].Clone();
    }
}