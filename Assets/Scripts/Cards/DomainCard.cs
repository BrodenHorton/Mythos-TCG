using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class DomainCard : Card {
    [SerializeField] private DomainCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public DomainCard() {
        cardType = CardType.Domain;
    }

    public DomainCard(DomainCardBase cardBase) {
        cardType = CardType.Domain;
        this.cardBase = cardBase;
        //effects = new List<SpellCardEffect>();
    }

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, SpellChainManager spellChainManager, MatchPlayer player) {
        if (!stateManager.CurrentState.CanPlaySetupCards())
            return false;
        if (spellChainManager.IsSpellChainActive())
            return false;
        if (player.CurrentMana < cardBase.ManaCost)
            return false;
        if (player.Domain != null)
            return false;

        return true;
    }

    public override void PlayCard(MatchPlayer player) {

    }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {
        EventBus.Instance.InvokeOnDomainCardSelectedForPlay(new PlayCardFromHandEventArgs<DomainCard>(player, this, handIndex));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        FixedString128Bytes uuidStr = serializer.IsWriter ? new FixedString128Bytes(uuid.ToString()) : new FixedString128Bytes();
        serializer.SerializeValue(ref uuidStr);
        if(serializer.IsReader)
            uuid = Guid.Parse(uuidStr.ToString());

        int cardBaseIndex = serializer.IsWriter ? CardDatabase.Instance.GetIndexOf(cardBase) : -1;
        serializer.SerializeValue(ref cardBaseIndex);
        if(serializer.IsReader)
            cardBase = CardDatabase.Instance.GetDomainCardByIndex(cardBaseIndex);
    }

    public string CardName { get { return cardBase.CardName; } }
}
