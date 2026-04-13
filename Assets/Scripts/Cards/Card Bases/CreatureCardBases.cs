using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureCardBases", menuName = "Scriptable Objects/Card List/CreatureCardList")]
public class CreatureCardBases : ScriptableObject {
    [SerializeField] private List<CreatureCardBase> cards;

    public List<CreatureCardBase> Cards { get { return cards; } }
}
