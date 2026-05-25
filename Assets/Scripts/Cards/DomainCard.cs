using System;
using System.Collections.Generic;
using UnityEngine;

public class DomainCard : Card {
    [SerializeField] private DomainCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public DomainCard(ulong playerId, DomainCardBase cardBase) : base(playerId) {
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

    public override void PlayCardFromHand(MatchPlayer player) {
        EventBus.Instance.InvokeOnDomainCardSelectedForPlay(new PlayerCardEventArgs<DomainCard>(player.PlayerId, this));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override CardPayload GetCardPayload() {
        return new DomainCardPayload(this);
    }

    public DomainCardBase CardBase { get { return cardBase; } }

    public string CardName { get { return cardBase.CardName; } }
}
