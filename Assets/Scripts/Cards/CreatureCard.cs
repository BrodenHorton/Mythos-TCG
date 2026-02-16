using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CreatureCard : Card {
    [SerializeField] private CreatureCardBase cardBase;
    [SerializeField] private List<CreatureCardEffect> effects;

    public CreatureCard() { }

    public CreatureCard(CreatureCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<CreatureCardEffect>();
    }

    public int GetAtk() {
        return cardBase.Atk;
    }

    public int GetHealth() {
        return cardBase.Health;
    }

    public int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override void Init(MatchPlayer player) {
        EventBus.InvokeOnCreatureCardDrawn(this, new DrawCreatureCardEventArgs(player, this));
    }

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
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