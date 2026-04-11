using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardBase", menuName = "Scriptable Objects/Card/SpellCardBase")]
public class SpellCardBase : CardBase {
    [SerializeField] private SpellType spellType;
    [SerializeReference, SubclassSelector] private List<SpellCardEffect> baseEffects;

    public override Card GenerateCardFromBase() {
        return new SpellCard(this);
    }

    public SpellType SpellType { get { return spellType; } }

    public List<SpellCardEffect> BaseEffects { get { return baseEffects; } }
}
