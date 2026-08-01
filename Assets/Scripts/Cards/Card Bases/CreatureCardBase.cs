using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureCardBase", menuName = "Scriptable Objects/Card/CreatureCardBase")]
public class CreatureCardBase : CardBase {
    [SerializeField] private int atk;
    [SerializeField] private int health;
    [SerializeField] private List<CreatureClass> creatureClasses;
    [SerializeField] private List<CreatureCardEffectBase> baseEffects;

    public override Card GenerateCardFromBase(ulong playerId) {
        return new CreatureCard(playerId, this);
    }

    public int Atk { get { return atk; } }

    public int Health { get { return health; } }

    public List<CreatureClass> CreatureClasses { get { return creatureClasses; } }

    public List <CreatureCardEffectBase> BaseEffects { get { return baseEffects; } }
}
