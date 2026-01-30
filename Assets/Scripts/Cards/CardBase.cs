using System.Collections.Generic;
using UnityEngine;

public class CardBase : ScriptableObject {
    [SerializeField] protected string cardName;
    [SerializeField] protected int manaCost;
    [SerializeField] protected Domain domain;
}
