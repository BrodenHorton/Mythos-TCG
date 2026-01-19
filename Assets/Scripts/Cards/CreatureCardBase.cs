using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CreatureCardBase", menuName = "Scriptable Objects/Card/CreatureCardBase")]
public class CreatureCardBase : CardBase {
    [SerializeField] int atk;
    [SerializeField] int health;
    [SerializeField] List<CreatureClass> creatureClasses;
}
