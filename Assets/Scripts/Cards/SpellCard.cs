using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public SpellCard() { }

    public SpellCard(SpellCardBase cardBase) {
        this.cardBase = cardBase;
        //effects = new List<SpellCardEffect>();
    }

    public SpellCard(SpellCardNetworkSerializable networkSerializationObject) {
        uuid = Guid.Parse(networkSerializationObject.uuidStr.ToString());
        cardBase = CardDatabase.Instance.GetSpellCardByIndex(networkSerializationObject.cardBaseIndex);
    }

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, MatchPlayer player) {
        if (cardBase.SpellType == SpellType.Instant) {
            if (!stateManager.CurrentState.CanPlayCombatCards())
                return false;
        }
        else {
            if (!stateManager.CurrentState.CanPlaySetupCards())
                return false;
        }
        if (player.CurrentMana < cardBase.ManaCost)
            return false;

        return true;
    }

    public override void PlayCard(MatchPlayer player) {
        
    }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {
        if (cardBase.SpellType == SpellType.Domain)
            EventBus.InvokeOnDomainCardSelectedForPlay(this, new PlaySpellCardFromHandEventArgs(player, this, handIndex));
        else
            EventBus.InvokeOnSpellCardSelectedForPlay(this, new PlaySpellCardFromHandEventArgs(player, this, handIndex));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public SpellCardNetworkSerializable GetNetworkSerializableObject() {
        return new SpellCardNetworkSerializable(uuid.ToString(), CardDatabase.Instance.GetIndexOf(cardBase));
    }

    public string CardName { get { return cardBase.CardName; } }
}