using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DomainCardBase", menuName = "Scriptable Objects/Card/DomainCardBase")]
public class DomainCardBase : CardBase {
    [SerializeReference, SubclassSelector] private List<SpellCardEffect> baseEffects;

    public override Card GenerateCardFromBase(ulong playerId) {
        return new DomainCard(playerId, this);
    }

    public List<SpellCardEffect> BaseEffects { get { return baseEffects; } }
}