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
        if (!stateManager.CurrentState.CanPlaySetupCards()) {
            if (!stateManager.CurrentState.CanDeclareCombatants())
                return false;
            else if (cardBase.SpellType != SpellType.Instant)
                return false;
        }
        if (player.CurrentMana < cardBase.ManaCost)
            return false;

        return true;
    }

    public override void PlayCard(MatchPlayer player) {
        
    }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {
        EventBus.InvokeOnSpellCardSelectedForPlay(this, new PlayCardFromHandEventArgs<SpellCard>(player, this, handIndex));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public SpellCardNetworkSerializable GetNetworkSerializableObject() {
        return new SpellCardNetworkSerializable(uuid.ToString(), CardDatabase.Instance.GetIndexOf(cardBase));
    }

    public string CardName { get { return cardBase.CardName; } }
}