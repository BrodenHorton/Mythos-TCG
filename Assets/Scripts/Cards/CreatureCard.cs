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

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
        if (player == null)
            Debug.Log("Player null");
        if (cardBase == null)
            Debug.Log("Card Base null");

        if (player.CurrentMana < cardBase.ManaCost)
            return false;
        if (player.Creatures.Count > 6)
            return false;

        return true;
    }

    public override void PlayCard(DuelManager duelManager, MatchPlayer player) {
        duelManager.SetCurrentMana(player, player.CurrentMana - cardBase.ManaCost);
        duelManager.PlayCreatureCard(duelManager.GetCurrentPlayerTurn(), this);
    }
}