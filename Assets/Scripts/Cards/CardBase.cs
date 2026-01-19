using System.Collections.Generic;
using UnityEngine;

public class CardBase : ScriptableObject {
    [SerializeField] string cardName;
    [SerializeField] int manaCost;
    [SerializeField] Domain domain;
    [SerializeField] List<string> baseEffects;
}
