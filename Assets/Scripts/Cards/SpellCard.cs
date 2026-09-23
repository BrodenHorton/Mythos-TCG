using System;
using UnityEngine;

[Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    //[SerializeField] private List<SpellCardEffect> effects;

    public SpellCard(ulong playerId, SpellCardBase cardBase) : base(playerId) {
        this.cardBase = cardBase;
        //effects = new List<SpellCardEffect>();
    }

    public void ExecuteSpell() {
        for (int i = 0; i < cardBase.BaseEffects.Count; i++) {
            // TODO: Make it so spell cards are executable
            //cardBase.BaseEffects[i].Execute();
            // TODO: Execute the additional effects on the SpellCard class
        }
    }

    public override bool IsPlayable(DuelManager duelManager,
                                    DuelStateManager stateManager,
                                    SpellChainManager spellChainManager,
                                    MatchPlayer player) {
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

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override CardBase GetCardBase() {
        return cardBase;
    }

    public override CardPayload GetCardPayload() {
        return new SpellCardPayload(this);
    }

    public SpellCardBase CardBase { get { return cardBase; } }

    public string CardName { get { return cardBase.CardName; } }

    public Material SplashArt { get { return cardBase.SplashArt; } }

    public SpellType SpellType { get { return cardBase.SpellType; } }
}