using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpellCardBases", menuName = "Scriptable Objects/Card List/SpellCardList")]
public class SpellCardBases : ScriptableObject {
    [SerializeField] private List<SpellCardBase> cards;

    public List<SpellCardBase> Cards { get { return cards; } }
}
