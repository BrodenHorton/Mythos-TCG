using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardBase", menuName = "Scriptable Objects/Card/SpellCardBase")]
public class SpellCardBase : CardBase {
    [SerializeField] SpellType spellType;
    [SerializeReference, SubclassSelector] private List<SpellCardEffect> baseEffects;
}
