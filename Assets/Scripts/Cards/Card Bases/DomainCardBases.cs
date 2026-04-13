using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DomainCardBases", menuName = "Scriptable Objects/Card List/DomainCardList")]
public class DomainCardBases : ScriptableObject {
    [SerializeField] private List<DomainCardBase> cards;

    public List<DomainCardBase> Cards { get { return cards; } }
}