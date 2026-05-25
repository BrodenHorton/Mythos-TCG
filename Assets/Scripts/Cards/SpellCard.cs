using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public SpellCard(ulong playerId, SpellCardBase cardBase) : base(playerId) {
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

    public override void PlayCardFromHand(MatchPlayer player) {
        EventBus.Instance.InvokeOnSpellCardSelectedForPlay(new PlayerCardEventArgs<SpellCard>(player.PlayerId, this));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override CardPayload GetCardPayload() {
        return new SpellCardPayload(this);
    }

    public SpellCardBase CardBase { get { return cardBase; } }

    public string CardName { get { return cardBase.CardName; } }

    public Material SplashArt { get { return cardBase.SplashArt; } }

    public SpellType SpellType { get { return cardBase.SpellType; } }

    public List<SpellCardEffect> BaseEffects { get { return cardBase.BaseEffects; } }
}