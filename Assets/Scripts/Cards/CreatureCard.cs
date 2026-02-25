using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CreatureCard : Card {
    [SerializeField] private CreatureCardBase cardBase;
    [SerializeField] private bool hasSummoningSickness;
    [SerializeField] private bool isTapped;
    [SerializeField] private int damage;
    [SerializeField] private List<CreatureCardEffect> effects;

    public CreatureCard() {}

    public CreatureCard(CreatureCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<CreatureCardEffect>();
    }

    public override void Init(MatchPlayer player) {
        EventBus.InvokeOnCreatureCardDrawn(this, new PlayerCreatureCardEventArgs(player, this));
    }

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
        if (player.CurrentMana < cardBase.ManaCost)
            return false;
        if (player.Creatures.Count > 6)
            return false;

        return true;
    }

    public override void PlayCard(DuelManager duelManager, MatchPlayer player) {
        // TODO: Implement logic for checking criteria for playing the card on the field
        player.PlayCreatureCard(this);
    }

    public int GetManaCost() {
        return cardBase.ManaCost;
    }

    public int GetAtk() {
        return cardBase.Atk;
    }

    public int GetHealth() {
        return cardBase.Health - damage;
    }

    public void Tap() {
        isTapped = true;
        EventBus.InvokeOnCreatureTapped(this, new CreatureCardEventArgs(this)); 
    }

    public void Untap() {
        isTapped = false;
        EventBus.InvokeOnCreatureUntapped(this, new CreatureCardEventArgs(this));
    }

    public bool CanAttack() {
        return !isTapped && !hasSummoningSickness;
    }

    public bool CanDefend() {
        return !isTapped;
    }

    public void Damage(int amt) {
        damage += amt;
    }

    public bool HasSummoningSickness { get { return hasSummoningSickness; } }

    public bool IsTapped { get { return isTapped; } }
}