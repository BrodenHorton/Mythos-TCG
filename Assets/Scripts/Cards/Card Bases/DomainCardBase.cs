using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DomainCardBase", menuName = "Scriptable Objects/Card/DomainCardBase")]
public class DomainCardBase : CardBase {
    [SerializeReference, SubclassSelector] private List<DomainCardEffectType> baseEffects;

    public override Card GenerateCardFromBase(ulong playerId) {
        return new DomainCard(playerId, this);
    }

    public List<DomainCardEffectType> BaseEffects { get { return baseEffects; } }
}