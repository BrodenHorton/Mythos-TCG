using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public SpellCard() { }

    public SpellCard(SpellCardNetworkSerializable networkSerializationObject) {
        uuid = Guid.Parse(networkSerializationObject.uuidStr.ToString());
        cardBase = CardDatabase.Instance.GetSpellCardByIndex(networkSerializationObject.cardBaseIndex);
    }

    public SpellCard(SpellCardBase cardBase) {
        this.cardBase = cardBase;
        //effects = new List<SpellCardEffect>();
    }

    public int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override void Init(MatchPlayer player) {
        EventBus.InvokeOnSpellCardDrawn(this, new PlayerSpellCardEventArgs(player, this));
    }

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
        if (player.CurrentMana < cardBase.ManaCost)
            return false;
        if (player.Creatures.Count > 6)
            return false;

        return true;
    }

    public override void PlayCard(DuelManager duelManager, MatchPlayer player) {
        
    }

    public override void PlayCardFromHand(DuelManager duelManager, MatchPlayer player, int handIndex) {
        
    }

    public SpellCardNetworkSerializable GetNetworkSerializableObject() {
        return new SpellCardNetworkSerializable(uuid.ToString(), CardDatabase.Instance.GetIndexOf(cardBase));
    }

    public string CardName { get { return cardBase.CardName; } }
}