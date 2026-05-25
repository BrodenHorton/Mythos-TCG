using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[Serializable]
public partial class CreatureCard : Card {
    [SerializeField] private CreatureCardBase cardBase;
    [SerializeField] private bool hasSummoningSickness;
    [SerializeField] private bool isTapped;
    [SerializeField] private int damage;
    [SerializeField] private List<CreatureCardEffect> effects;

    private Action<CreatureCard> creatureHealthChangedCallback;
    private Action<CreatureCard> creatureDamagedCallback;
    private Action<CreatureCard> creatureDestroyedCallback;

    public CreatureCard(CreatureCardBase cardBase) {
        this.cardBase = cardBase;
        effects = new List<CreatureCardEffect>();
        for(int i = 0; i < cardBase.BaseEffects.Count; i++) {
            CreatureCardEffect effect = cardBase.BaseEffects[i].DeepCopy();
            effect.Init(uuid);
            effects.Add(effect);
        }
    }

    public override bool IsPlayable(DuelManager duelManager, DuelStateManager stateManager, SpellChainManager spellChainManager, MatchPlayer player) {
        if (!stateManager.CurrentState.CanPlaySetupCards())
            return false;
        if (spellChainManager.IsSpellChainActive())
            return false;
        if (player.CurrentMana < cardBase.ManaCost)
            return false;
        if (player.Creatures.Count > 6)
            return false;

        return true;
    }

    // TODO: Implement for cards not played from the hand
    public override void PlayCard(MatchPlayer player) {
        //EventBus.InvokeOnCreatureCardPlayed(this, new PlayCreatureCardFromHandEventArgs(player, this));
    }

    public override void PlayCardFromHand(MatchPlayer player) {
        EventBus.Instance.InvokeOnCreatureCardSelectedForPlay(new PlayerCardEventArgs<CreatureCard>(player.PlayerId, this));
    }

    public override int GetManaCost() {
        return cardBase.ManaCost;
    }

    public int GetAtk() {
        return cardBase.Atk;
    }

    public int GetHealth() {
        return cardBase.Health - damage;
    }

    public void Tap() {
        isTapped = true;
        EventBus.Instance.InvokeOnCreatureTapped(new CardPayloadEventArgs<CreatureCardPayload>(new CreatureCardPayload(this))); 
    }

    public void Untap() {
        isTapped = false;
        EventBus.Instance.InvokeOnCreatureUntapped(new CardPayloadEventArgs<CreatureCardPayload>(new CreatureCardPayload(this)));
    }

    public bool CanAttack() {
        return !isTapped && !hasSummoningSickness;
    }

    public bool CanDefend() {
        return !isTapped;
    }

    public void InflictDamage(int amt) {
        damage += amt;
        creatureDamagedCallback?.Invoke(this);
        if(GetHealth() <= 0)
            creatureDestroyedCallback?.Invoke(this);
    }

    public override CardPayload GetCardPayload() {
        return new CreatureCardPayload(this);
    }

    public CreatureCardBase CardBase { get {  return cardBase; } }

    public string CardName { get { return cardBase.CardName; } }

    public Material SplashArt { get { return cardBase.SplashArt; } }

    public int BaseManaCost { get { return cardBase.ManaCost; } }

    public int BaseAtk { get { return cardBase.Atk; } }

    public int BaseHealth { get { return cardBase.Health; } }

    // Might want to make a callback method for summoning sickness since the event needs to know the player id
    public bool HasSummoningSickness { get { return hasSummoningSickness; } set { hasSummoningSickness = value; } }

    public bool IsTapped { get { return isTapped; } }

    public int CurrentDamage {
        get {
            return damage;
        }
        set {
            if(damage != value) {
                damage = value;
                creatureHealthChangedCallback?.Invoke(this);
            }
        }
    }

    public List<CreatureCardEffect> Effects { get { return effects; } }

    public Action<CreatureCard> CreatureHealthChangedCallback { get { return creatureDamagedCallback; } set { creatureHealthChangedCallback = value; } }

    public Action<CreatureCard> CreatureDamagedCallback { get { return creatureDamagedCallback; } set { creatureDamagedCallback = value; } }

    public Action<CreatureCard> CreatureDestroyedCallback { get { return creatureDestroyedCallback; } set { creatureDestroyedCallback = value; } }
}