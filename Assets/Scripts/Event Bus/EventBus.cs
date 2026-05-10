using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EventBus : NetworkBehaviour {
    // Duelist UI Actions
    public event EventHandler<PlayerCardEventArgs<Card>> OnCardDrawn;
    public event EventHandler<CardRemovedFromHandEventArgs> OnCardRemovedFromHand;
    // PlayerUI Card Drag
    public event EventHandler<HandCardDragEventArgs> OnStartHandCardDrag;
    public event EventHandler<HandCardDragEventArgs> OnReleaseHandCardDrag;
    public event EventHandler<PlayHandCardEventArgs> OnPlayHandCard;
    // PlayingField Card Drag
    public event EventHandler<FieldCardDragEventArgs<CreatureFieldCardUI>> OnStartCardDragPlayingField;
    public event EventHandler<ReleaseFieldCardDragEventArgs<CreatureFieldCardUI>> OnReleaseCardDragPlayingField;
    public event EventHandler<CreatureFieldCardEnteringCombatFieldEventArgs> OnReleaseCreatureFieldCardOverCombatArea;
    // Playing Cards
    public event EventHandler<PlayCardFromHandEventArgs<CreatureCard>> OnCreatureCardSelectedForPlay;
    public event EventHandler<PlayCardFromHandEventArgs<CreatureCard>> OnCreatureCardPlayedFromHand;
    public event EventHandler<PlayCardFromHandEventArgs<SpellCard>> OnSpellCardSelectedForPlay;
    public event EventHandler<PlayCardFromHandEventArgs<SpellCard>> OnSpellCardPlayedFromHand;
    public event EventHandler<PlayCardFromHandEventArgs<DomainCard>> OnDomainCardSelectedForPlay;
    public event EventHandler<PlayCardFromHandEventArgs<DomainCard>> OnDomainCardPlayedFromHand;
    public event EventHandler<PlayerCardEventArgs<SpellCard>> OnSpellChainCardPlayed; // This event should only ever be invoked by the server
    // Player Status Changes
    public event EventHandler<LifePointsChangedEventArgs> OnLifePointsChanged;
    public event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    // Creature Status Changes
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureHealthChanged;
    // Declaring and Undeclaring creatures
    public event EventHandler<DeclareAttackerEventArgs> OnDeclareAttacker;
    public event EventHandler<DeclareDefenderEventArgs> OnDeclareDefender;
    public event EventHandler<UndeclareAttackerEventArgs> OnUndeclareAttacker;
    public event EventHandler<UndeclareDefenderEventArgs> OnUndeclareDefender;
    public event EventHandler<SelectAttackerToDefendEventArgs> OnSelectAttackerToDefend;
    // Combat
    public event EventHandler<CreatureAttackEventArgs> OnCreatureAttack;
    public event EventHandler<CreatureDamagedByCreatureEventArgs> OnCreatureDamagedByCreature;
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureDamaged;
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureDestroyed;
    public event EventHandler<ReleaseCombatCreaturesEventArgs> OnReleaseCombatCreatures;
    // Creature Actions
    public event EventHandler<CreatureCardEventArgs> OnCreatureTapped;
    public event EventHandler<CreatureCardEventArgs> OnCreatureUntapped;

    public static EventBus Instance { get; private set; }

    private DuelManager duelManager;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("EventBus already exists in scene. Destroying redundant object.");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
    }

    #region Duelist UI Actions
    public void InvokeOnCardDrawn(ulong playerId, Card card) {
        if (!IsServer)
            return;

        List<ulong> otherPlayerIds = duelManager.GetPlayerIds();
        otherPlayerIds.Remove(playerId);
        BaseRpcTarget playerTarget = RpcTarget.Group(new List<ulong>() { playerId }, RpcTargetUse.Temp);
        CardNetworkContainer cardNetworkContainer = new CardNetworkContainer();
        cardNetworkContainer.card = card;
        InvokeOnCardDrawnClientRpc(playerId, cardNetworkContainer, playerTarget);
        BaseRpcTarget otherTarget = RpcTarget.Group(otherPlayerIds, RpcTargetUse.Temp);
        CardNetworkContainer nullCardNetworkContainer = new CardNetworkContainer();
        cardNetworkContainer.card = new NullCard();
        InvokeOnCardDrawnClientRpc(playerId, nullCardNetworkContainer, otherTarget);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InvokeOnCardDrawnClientRpc(ulong playerId, CardNetworkContainer cardNetworkContainer, RpcParams rpcParams) {
        Card card = cardNetworkContainer.card;
        Instance.OnCardDrawn?.Invoke(this, new PlayerCardEventArgs<Card>(playerId, card));
    }

    public void InvokeOnCardRemovedFromHand(CardRemovedFromHandEventArgs args) {
        OnCardRemovedFromHand?.Invoke(this, args);
    }
    #endregion

    #region PlayerUI Card Drag
    public void InvokeOnStartHandCardDrag(HandCardDragEventArgs args) {
        OnStartHandCardDrag?.Invoke(this, args);
    }

    public void InvokeOnReleaseHandCardDrag(HandCardDragEventArgs args) {
        OnReleaseHandCardDrag?.Invoke(this, args);
    }

    public void InvokeOnPlayHandCard(PlayHandCardEventArgs args) {
        OnPlayHandCard?.Invoke(this, args);
    }
    #endregion

    #region PlayingField Card Drag
    public void InvokeOnStartCardDragPlayingField(FieldCardDragEventArgs<CreatureFieldCardUI> args) {
        OnStartCardDragPlayingField?.Invoke(this, args);
    }

    public void InvokeOnReleaseCardDragPlayingField(ReleaseFieldCardDragEventArgs<CreatureFieldCardUI> args) {
        OnReleaseCardDragPlayingField?.Invoke(this, args);
    }

    public void InvokeOnReleaseCreatureFieldCardOverCombatArea(CreatureFieldCardEnteringCombatFieldEventArgs args) {
        OnReleaseCreatureFieldCardOverCombatArea?.Invoke(this, args);
    }
    #endregion

    #region Playing Cards
    public void InvokeOnCreatureCardSelectedForPlay(PlayCardFromHandEventArgs<CreatureCard> args) {
        OnCreatureCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnCreatureCardPlayedFromHand(PlayCardFromHandEventArgs<CreatureCard> args) {
        OnCreatureCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnSpellCardSelectedForPlay(PlayCardFromHandEventArgs<SpellCard> args) {
        OnSpellCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnSpellCardPlayedFromHand(PlayCardFromHandEventArgs<SpellCard> args) {
        OnSpellCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnDomainCardSelectedForPlay(PlayCardFromHandEventArgs<DomainCard> args) {
        OnDomainCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnDomainCardPlayedFromHand(PlayCardFromHandEventArgs<DomainCard> args) {
        OnDomainCardPlayedFromHand?.Invoke(this, args);
    }

    // This event should only ever be invoked by the server
    public void InvokeOnSpellChainCardPlayed(PlayerCardEventArgs<SpellCard> args) {
        OnSpellChainCardPlayed?.Invoke(this, args);
    }
    #endregion

    #region Player Status Changes
    public void InvokeOnLifePointsChanged(ulong playerId, int lifePoints) {
        if (!IsServer)
            return;

        InvokeOnLifePointsChangedClientRpc(playerId, lifePoints);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InvokeOnLifePointsChangedClientRpc(ulong playerId, int lifePoints) {
        LifePointsChangedEventArgs args = new LifePointsChangedEventArgs(playerId, lifePoints);
        OnLifePointsChanged?.Invoke(this, args);
    }

    public void InvokeOnManaCountChanged(ulong playerId, int manaCount) {
        if (!IsServer)
            return;

        InvokeOnManaCountChangedClientRpc(playerId, manaCount);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnManaCountChangedClientRpc(ulong playerId, int manaCount) {
        ManaChangedEventArgs args = new ManaChangedEventArgs(playerId, manaCount);
        OnManaCountChanged?.Invoke(this, args);
    }
    #endregion

    #region Creature Status Changes
    public void InvokeOnCreatureHealthChanged(PlayerCardEventArgs<CreatureCard> args) {
        OnCreatureHealthChanged?.Invoke(this, args);
    }
    #endregion

    #region Declaring and Undeclaring Creatures
    public void InvokeOnDeclareAttacker(DeclareAttackerEventArgs args) {
        OnDeclareAttacker?.Invoke(this, args);
    }

    public void InvokeOnDeclareDefender(DeclareDefenderEventArgs args) {
        OnDeclareDefender?.Invoke(this, args);
    }

    public void InvokeOnUndelcareAttacker(UndeclareAttackerEventArgs args) {
        OnUndeclareAttacker?.Invoke(this, args);
    }

    public void InvokeOnUndeclareDefender(UndeclareDefenderEventArgs args) {
        OnUndeclareDefender?.Invoke(this, args);
    }

    public void InvokeOnSelectAttackerToDefend(SelectAttackerToDefendEventArgs args) {
        OnSelectAttackerToDefend?.Invoke(this, args);
    }
    #endregion

    #region Combat
    public void InvokeOnCreatureAttack(CreatureAttackEventArgs args) {
        OnCreatureAttack?.Invoke(this, args);
    }

    public void InvokeOnCreatureDamagedByCreature(CreatureDamagedByCreatureEventArgs args) {
        OnCreatureDamagedByCreature?.Invoke(this, args);
    }

    public void InvokeOnCreatureDamaged(PlayerCardEventArgs<CreatureCard> args) {
        OnCreatureDamaged?.Invoke(this, args);
    }

    public void InvokeOnCreatureDestroyed(PlayerCardEventArgs<CreatureCard> args) {
        OnCreatureDestroyed?.Invoke(this, args);
    }

    public void InvokeOnReleaseCombatCreatures(ReleaseCombatCreaturesEventArgs args) {
        OnReleaseCombatCreatures?.Invoke(this, args);
    }
    #endregion

    #region Creature Actions
    public void InvokeOnCreatureTapped(CreatureCardEventArgs args) {
        OnCreatureTapped?.Invoke(this, args);
    }

    public void InvokeOnCreatureUntapped(CreatureCardEventArgs args) {
        OnCreatureUntapped?.Invoke(this, args);
    }
    #endregion
}
