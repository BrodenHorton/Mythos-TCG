using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    [SerializeField] private List<SpellCardEffect> effects;

    public SpellCard(SpellCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<SpellCardEffect>();
    }

    public override void DisplayCard() {
        throw new System.NotImplementedException();
    }

    public override bool IsPlayable() {
        throw new System.NotImplementedException();
    }
}