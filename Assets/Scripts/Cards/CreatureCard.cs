using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
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

    public CreatureCard() {
        cardType = CardType.Creature;
    }

    public CreatureCard(CreatureCardBase cardBase) {
        cardType = CardType.Creature;
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

    // TODO: Implement for cards not payed from the hand
    public override void PlayCard(MatchPlayer player) {
        //EventBus.InvokeOnCreatureCardPlayed(this, new PlayCreatureCardFromHandEventArgs(player, this));
    }

    public override void PlayCardFromHand(MatchPlayer player, int handIndex) {
        EventBus.Instance.InvokeOnCreatureCardSelectedForPlay(new PlayCardFromHandEventArgs<CreatureCard>(player, this, handIndex));
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
        EventBus.Instance.InvokeOnCreatureTapped(new CreatureCardEventArgs(this)); 
    }

    public void Untap() {
        isTapped = false;
        EventBus.Instance.InvokeOnCreatureUntapped(new CreatureCardEventArgs(this));
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

    public override void NetworkSerialize<T>(BufferSerializer<T> serializer) {
        FixedString128Bytes uuidStr = serializer.IsWriter ? new FixedString128Bytes(uuid.ToString()) : new FixedString128Bytes();
        serializer.SerializeValue(ref uuidStr);
        if (serializer.IsReader)
            uuid = Guid.Parse(uuidStr.ToString());

        int cardBaseIndex = serializer.IsWriter ? CardDatabase.Instance.GetIndexOf(cardBase) : -1;
        serializer.SerializeValue(ref cardBaseIndex);
        if (serializer.IsReader)
            cardBase = CardDatabase.Instance.GetCreatureCardByIndex(cardBaseIndex);

        serializer.SerializeValue(ref hasSummoningSickness);
        serializer.SerializeValue(ref isTapped);
        serializer.SerializeValue(ref damage);

        CreatureCardEffectNetworkContainer effectContainer = serializer.IsWriter ? new CreatureCardEffectNetworkContainer(effects.ToArray()) : new CreatureCardEffectNetworkContainer();
        serializer.SerializeNetworkSerializable(ref effectContainer);
        if(serializer.IsReader) {
            effects = new List<CreatureCardEffect>();
            for (int i = 0; i < effectContainer.effects.Length; i++) {
                CreatureCardEffect effect = effectContainer.effects[i];
                effect.Init(uuid);
                effects.Add(effect);
            }
        }
    }

    public string CardName { get { return cardBase.CardName; } }

    public Material SplashArt { get { return cardBase.SplashArt; } }

    public int BaseManaCost { get { return cardBase.ManaCost; } }

    public int BaseAtk { get { return cardBase.Atk; } }

    public int BaseHealth { get { return cardBase.Health; } }

    public bool HasSummoningSickness { get { return hasSummoningSickness; } }

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

    public Action<CreatureCard> CreatureHealthChangedCallback { get { return creatureDamagedCallback; } set { creatureHealthChangedCallback = value; } }

    public Action<CreatureCard> CreatureDamagedCallback { get { return creatureDamagedCallback; } set { creatureDamagedCallback = value; } }

    public Action<CreatureCard> CreatureDestroyedCallback { get { return creatureDestroyedCallback; } set { creatureDestroyedCallback = value; } }
}
