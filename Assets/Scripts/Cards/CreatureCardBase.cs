using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureCardBase", menuName = "Scriptable Objects/Card/CreatureCardBase")]
public class CreatureCardBase : CardBase {
    [SerializeField] private int atk;
    [SerializeField] private int health;
    [SerializeField] private List<CreatureClass> creatureClasses;
    [SerializeReference, SubclassSelector] private List<CreatureCardEffect> baseEffects;
}
