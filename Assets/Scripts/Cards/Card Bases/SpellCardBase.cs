using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardBase", menuName = "Scriptable Objects/Card/SpellCardBase")]
public class SpellCardBase : CardBase {
    [SerializeField] private SpellType spellType;
    [SerializeReference, SubclassSelector] private List<SpellCardEffectType> baseEffects;

    public override Card GenerateCardFromBase(ulong playerId) {
        return new SpellCard(playerId, this);
    }

    public SpellType SpellType { get { return spellType; } }

    public List<SpellCardEffectType> BaseEffects { get { return baseEffects; } }
}
