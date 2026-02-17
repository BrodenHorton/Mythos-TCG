using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CreatureCard : Card {
    [SerializeField] private CreatureCardBase cardBase;
    [SerializeField] private bool hasSummoningSickness;
    [SerializeField] private bool isTapped;
    [SerializeField] private List<CreatureCardEffect> effects;

    public CreatureCard() { }

    public CreatureCard(CreatureCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<CreatureCardEffect>();
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

    public int GetManaCost() {
        return cardBase.ManaCost;
    }

    public int GetAtk() {
        return cardBase.Atk;
    }

    public int GetHealth() {
        return cardBase.Health;
    }

    public void Tap() {

    }

    public void Untap() {

    }

    public bool CanAttack() {
        return false;
    }

    public bool CanDefend() {
        return false;
    }

    public bool HasSummoningSickness { get { return hasSummoningSickness; } }

    public bool IsTapped { get { return isTapped; } }
}