using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CardBase : ScriptableObject {
    [SerializeField] protected string cardName;
    [SerializeField] Image splashArt;
    [SerializeField] protected int manaCost;
    [SerializeField] protected Domain domain;
}
