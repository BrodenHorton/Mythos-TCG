using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public SpellCard() {
        cardType = CardType.Spell;
    }

    public SpellCard(SpellCardBase cardBase) {
        cardType = CardType.Spell;
        this.cardBase = cardBase;
        //effects = new List<SpellCardEffect>();
    }

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, SpellChainManager spellChainManager, MatchPlayer player) {
        if(spellChainManager.IsSpellChainActive()) {
            if (SpellType == SpellType.Slow)
                return false;
        }
        else {
            if (!stateManager.CurrentState.CanPlaySpellCards())
                return false;
        }
        if (player.CurrentMana < cardBase.ManaCost)
            return false;

        return true;
    }

    public override void PlayCard(MatchPlayer player) {
        
    }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {
        EventBus.Instance.InvokeOnSpellCardSelectedForPlay(new PlayCardFromHandEventArgs<SpellCard>(player, this, handIndex));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        FixedString128Bytes uuidStr = serializer.IsWriter ? new FixedString128Bytes(uuid.ToString()) : new FixedString128Bytes();
        serializer.SerializeValue(ref uuidStr);
        if (serializer.IsReader)
            uuid = Guid.Parse(uuidStr.ToString());

        int cardBaseIndex = serializer.IsWriter ? CardDatabase.Instance.GetIndexOf(cardBase) : -1;
        serializer.SerializeValue(ref cardBaseIndex);
        if (serializer.IsReader)
            cardBase = CardDatabase.Instance.GetSpellCardByIndex(cardBaseIndex);
    }

    public string CardName { get { return cardBase.CardName; } }

    public Material SplashArt { get { return cardBase.SplashArt; } }

    public SpellType SpellType { get { return cardBase.SpellType; } }

    public List<SpellCardEffect> BaseEffects { get { return cardBase.BaseEffects; } }
}