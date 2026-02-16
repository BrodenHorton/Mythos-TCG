using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpellCard : Card {
    [SerializeField] private SpellCardBase cardBase;
    [SerializeField] private List<SpellCardEffect> effects;

    public SpellCard() { }

    public SpellCard(SpellCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<SpellCardEffect>();
    }

    public int GetManaCost() {
        return cardBase.ManaCost;
    }

    public override void Init(MatchPlayer player) {
        EventBus.InvokeOnSpellCardDrawn(this, new DrawCardEventArgs(player, this));
    }

    public override bool IsPlayable(DuelManager duelManager, MatchPlayer player) {
        if (player.CurrentMana < cardBase.ManaCost)
            return false;
        if (player.Creatures.Count > 6)
            return false;

        return true;
    }

    public override void PlayCard(DuelManager duelManager, MatchPlayer player) {
        duelManager.SetCurrentMana(player, player.CurrentMana - cardBase.ManaCost);
        duelManager.PlaySpellCard(duelManager.GetCurrentPlayerTurn(), this);
    }
}