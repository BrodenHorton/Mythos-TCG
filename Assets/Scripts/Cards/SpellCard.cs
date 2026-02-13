using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    [SerializeField] private List<SpellCardEffect> effects;

    public SpellCard() { }

    public SpellCard(SpellCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<SpellCardEffect>();
    }

    public override void DisplayCard() {
        throw new System.NotImplementedException();
    }

    public override bool IsPlayable(DuelManager duelManager) {
        System.Random rand = new System.Random();
        return rand.Next(0, 2) == 0;
    }

    public override void PlayCard(DuelManager duelManager) {
        duelManager.PlaySpellCard(duelManager.GetCurrentPlayerTurn(), this);
    }
}