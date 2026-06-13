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

    private Action<CreatureCard> creatureDestroyedCallback;

    public CreatureCard(ulong playerId, CreatureCardBase cardBase) : base(playerId) {
        this.cardBase = cardBase;
        effects = new List<CreatureCardEffect>();
        for(int i = 0; i < cardBase.BaseEffects.Count; i++) {
            CreatureCardEffect effect = cardBase.BaseEffects[i].DeepCopy();
            effect.Init(this);
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
        PlayerCardStatEventArgs<Card> args = new PlayerCardStatEventArgs<Card>(playerId, this, cardBase.ManaCost);
        EventBus.Instance.InvokeOnCalculateCardManaCount(args);
        return args.Value;
    }

    public int GetAtk() {
        PlayerCardStatEventArgs<CreatureCard> args = new PlayerCardStatEventArgs<CreatureCard>(playerId,
                                                                                               this,
                                                                                               cardBase.Atk);
        EventBus.Instance.InvokeOnCalculateCreatureAttack(args);
        return args.Value;
    }

    public int GetHealth() {
        PlayerCardStatEventArgs<CreatureCard> args = new PlayerCardStatEventArgs<CreatureCard>(playerId,
                                                                                               this,
                                                                                               cardBase.Health - damage);
        EventBus.Instance.InvokeOnCalculateCreatureHealth(args);
        return args.Value;
    }

    public void Tap() {
        if (isTapped)
            throw new Exception("Attempting to tap a creature that is already tapped");
        PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(playerId, this);
        EventBus.Instance.InvokeOnCreatureTapped(args);
        if (args.IsCanceled)
            return;

        isTapped = true;
        EventBus.Instance.InvokeOnCreatureTappedFinishedClientRpc(playerId, new CreatureCardPayload(this));
    }

    public void Untap() {
        if (!isTapped)
            throw new Exception("Attempting to untap a creature that isn't tapped");
        PlayerCardCancelableEventArgs<CreatureCard> args = new PlayerCardCancelableEventArgs<CreatureCard>(playerId, this);
        EventBus.Instance.InvokeOnCreatureUntapped(args);
        if (args.IsCanceled)
            return;

        isTapped = false;
        EventBus.Instance.InvokeOnCreatureUntappedFinishedClientRpc(playerId, new CreatureCardPayload(this));
    }

    public bool CanAttack() {
        return !isTapped && !hasSummoningSickness;
    }

    public bool CanDefend() {
        return !isTapped;
    }

    public void InflictDamage(int amt) {
        damage += amt;
        EventBus.Instance.InvokeOnCreatureDamaged(new PlayerCardEventArgs<CreatureCard>(playerId, this));
        EventBus.Instance.InvokeOnCreatureDamagedFinishedClientRpc(playerId, new CreatureCardPayload(this));
        CheckHealthState();
    }

    public void CheckHealthState() {
        if (GetHealth() <= 0)
            creatureDestroyedCallback?.Invoke(this);
    }

    public void AddEffect(CreatureCardEffect effect) {
        effect.Init(this);
        effects.Add(effect);
    }

    public void RemoveEffect(CreatureCardEffect effect) {
        foreach(CreatureCardEffect entry in effects) {
            if(entry == effect) {
                entry.RemoveListeners();
                effects.Remove(entry);
                return;
            }
        }

        throw new Exception("Unable to find specified CreatureCardEffect on CreatureCard");
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

    public bool HasSummoningSickness { get { return hasSummoningSickness; } set { hasSummoningSickness = value; } }

    public bool IsTapped { get { return isTapped; } }

    public int CurrentDamage { get { return damage; } set { damage = value; } }

    public List<CreatureCardEffect> Effects { get { return effects; } }

    public Action<CreatureCard> CreatureDestroyedCallback { get { return creatureDestroyedCallback; } set { creatureDestroyedCallback = value; } }
}