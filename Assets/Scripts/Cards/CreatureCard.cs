using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CreatureCard : Card {
    [SerializeField] private CreatureCardBase cardBase;
    [SerializeField] private List<CreatureCardEffect> effects;

    public CreatureCard() { }

    public CreatureCard(CreatureCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<CreatureCardEffect>();
    }

    public int GetAtk() {
        return 0;
    }

    public int GetHealth() {
        return 0;
    }

    public override void DisplayCard() {
        throw new System.NotImplementedException();
    }

    public override bool IsPlayable() {
        System.Random rand = new System.Random();
        return rand.Next(0, 2) == 0;
    }
}