using System;
using UnityEngine;

public class DomainCard : Card {
    [SerializeField] private DomainCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public DomainCard() { }

    public DomainCard(DomainCardBase cardBase) {
        this.cardBase = cardBase;
        //effects = new List<SpellCardEffect>();
    }

    public DomainCard(DomainCardNetworkSerializable networkSerializationObject) {
        uuid = Guid.Parse(networkSerializationObject.uuidStr.ToString());
        cardBase = CardDatabase.Instance.GetDomainCardByIndex(networkSerializationObject.cardBaseIndex);
    }

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, MatchPlayer player) {
        if (!stateManager.CurrentState.CanPlaySetupCards())
            return false;
        if (player.CurrentMana < cardBase.ManaCost)
            return false;

        return true;
    }

    public override void PlayCard(MatchPlayer player) {

    }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {
        EventBus.InvokeOnDomainCardSelectedForPlay(this, new PlayCardFromHandEventArgs<DomainCard>(player, this, handIndex));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public DomainCardNetworkSerializable GetNetworkSerializableObject() {
        return new DomainCardNetworkSerializable(uuid.ToString(), CardDatabase.Instance.GetIndexOf(cardBase));
    }

    public string CardName { get { return cardBase.CardName; } }
}
