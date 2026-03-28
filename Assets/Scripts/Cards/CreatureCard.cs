using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public partial class CreatureCard : Card {
    [SerializeField] private CreatureCardBase cardBase;
    [SerializeField] private bool hasSummoningSickness;
    [SerializeField] private bool isTapped;
    [SerializeField] private int damage;

    public CreatureCard() { }

    public CreatureCard(CreatureCardBase cardBase) {
        this.cardBase = cardBase;
        //effects = new List<CreatureCardEffect>();
    }

    public CreatureCard(CreatureCardNetworkSerializable networkSerializationObject) {
        uuid = Guid.Parse(networkSerializationObject.uuidStr.ToString());
        cardBase = CardDatabase.Instance.GetCreatureCardByIndex(networkSerializationObject.cardBaseIndex);
        hasSummoningSickness = networkSerializationObject.hasSummoningSickness;
        isTapped = networkSerializationObject.isTapped;
        damage = networkSerializationObject.damage;
    }

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
        if (player.CurrentMana < cardBase.ManaCost)
            return false;
        if (player.Creatures.Count > 6)
            return false;

        return true;
    }

    // TODO: Implement for cards not payed from the hand
    public override void PlayCard(MatchPlayer player) {
        //EventBus.InvokeOnCreatureCardPlayed(this, new PlayCreatureCardFromHandEventArgs(player, this));
    }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {
        EventBus.InvokeOnCreatureCardSelectedForPlay(this, new PlayCreatureCardFromHandEventArgs(player, this, handIndex));
    }

    public override int GetManaCost() {
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

    public CreatureCardNetworkSerializable GetNetworkSerializableObject() {
        return new CreatureCardNetworkSerializable(
            uuid.ToString(),
            CardDatabase.Instance.GetIndexOf(cardBase),
            hasSummoningSickness,
            isTapped,
            damage);
    }

    public string CardName { get { return cardBase.CardName; } }

    public bool HasSummoningSickness { get { return hasSummoningSickness; } }

    public bool IsTapped { get { return isTapped; } }
}
