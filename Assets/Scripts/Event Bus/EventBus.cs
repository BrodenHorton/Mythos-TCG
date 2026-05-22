using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EventBus : NetworkBehaviour {
    // Duelist UI Actions
    public event EventHandler<PlayerCardPayloadEventArgs<CardPayload>> OnCardDrawn;
    public event EventHandler<PlayerCardPayloadEventArgs<CardPayload>> OnCardRemovedFromHand;
    // PlayerUI Card Drag
    public event EventHandler<HandCardDragEventArgs> OnStartHandCardDrag;
    public event EventHandler<HandCardDragEventArgs> OnReleaseHandCardDrag;
    public event EventHandler<PlayerCardUuidEventArgs> OnPlayHandCard;
    // PlayingField Card Drag
    public event EventHandler<PlayingFieldCardEventArgs<CreatureFieldCardUI>> OnStartCardDragPlayingField;
    public event EventHandler<PlayingFieldCardEventArgs<CreatureFieldCardUI>> OnReleaseCardDragPlayingField;
    public event EventHandler<CombatFieldCardEventArgs<CreatureFieldCardUI>> OnReleaseCreatureFieldCardOverCombatArea;
    // Playing Cards
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureCardSelectedForPlay;
    public event EventHandler<PlayerCardEventArgs<DomainCard>> OnDomainCardSelectedForPlay;
    public event EventHandler<PlayerCardEventArgs<SpellCard>> OnSpellCardSelectedForPlay;
    public event EventHandler<PlayerCardPayloadEventArgs<CreatureCardPayload>> OnCreatureCardPlayedFromHand;
    public event EventHandler<PlayerCardPayloadEventArgs<DomainCardPayload>> OnDomainCardPlayedFromHand;
    public event EventHandler<PlayerCardPayloadEventArgs<SpellCardPayload>> OnSpellCardPlayedFromHand;
    public event EventHandler<PlayerCardEventArgs<SpellCard>> OnSpellChainCardPlayed;
    // Player Status Changes
    public event EventHandler<LifePointsChangedEventArgs> OnLifePointsChanged;
    public event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    // Creature Status Changes
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureHealed;
    // Declaring and Undeclaring creatures
    public event EventHandler<DeclareAttackerEventArgs> OnDeclareAttacker;
    public event EventHandler<DeclareDefenderEventArgs> OnDeclareDefender;
    public event EventHandler<UndeclareAttackerEventArgs> OnUndeclareAttacker;
    public event EventHandler<UndeclareAttackerEventArgs> OnUndeclareAttackerFinished;
    public event EventHandler<UndeclareDefenderEventArgs> OnUndeclareDefender;
    public event EventHandler<UndeclareDefenderEventArgs> OnUndeclareDefenderFinished;
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
        duelManager = ServiceLocator.Get<DuelManager>();
    }

    #region Duelist UI Actions
    public void InvokeOnCardDrawn(ulong playerId, Card card) {
        if (!IsServer)
            return;

        List<ulong> otherPlayerIds = duelManager.GetPlayerIds();
        otherPlayerIds.Remove(playerId);
        BaseRpcTarget playerTarget = RpcTarget.Single(playerId, RpcTargetUse.Temp);
        CardPayloadNetworkContainer cardPayloadNetworkContainer = new CardPayloadNetworkContainer();
        cardPayloadNetworkContainer.cardPayload = card.GetCardPayload();
        InvokeOnCardDrawnClientRpc(playerId, cardPayloadNetworkContainer, playerTarget);
        BaseRpcTarget otherTarget = RpcTarget.Group(otherPlayerIds, RpcTargetUse.Temp);
        CardPayloadNetworkContainer nullCardPayloadNetworkContainer = new CardPayloadNetworkContainer();
        nullCardPayloadNetworkContainer.cardPayload = new NullCardPayload();
        InvokeOnCardDrawnClientRpc(playerId, nullCardPayloadNetworkContainer, otherTarget);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InvokeOnCardDrawnClientRpc(ulong playerId, CardPayloadNetworkContainer cardNetworkContainer, RpcParams rpcParams) {
        Instance.OnCardDrawn?.Invoke(this, new PlayerCardPayloadEventArgs<CardPayload>(playerId, cardNetworkContainer.cardPayload));
    }

    public void InvokeOnCardRemovedFromHand(ulong playerId, Card card) {
        if (!IsServer)
            return;

        List<ulong> otherPlayerIds = duelManager.GetPlayerIds();
        otherPlayerIds.Remove(playerId);
        BaseRpcTarget playerTarget = RpcTarget.Single(playerId, RpcTargetUse.Temp);
        CardPayloadNetworkContainer cardPayloadNetworkContainer = new CardPayloadNetworkContainer();
        cardPayloadNetworkContainer.cardPayload = card.GetCardPayload();
        InvokeOnCardRemovedFromHandClientRpc(playerId, cardPayloadNetworkContainer, playerTarget);
        BaseRpcTarget otherTarget = RpcTarget.Group(otherPlayerIds, RpcTargetUse.Temp);
        CardPayloadNetworkContainer nullCardPayloadNetworkContainer = new CardPayloadNetworkContainer();
        nullCardPayloadNetworkContainer.cardPayload = new NullCardPayload();
        InvokeOnCardRemovedFromHandClientRpc(playerId, nullCardPayloadNetworkContainer, otherTarget);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void InvokeOnCardRemovedFromHandClientRpc(ulong playerId, CardPayloadNetworkContainer cardNetworkContainer, RpcParams rpcParams) {
        OnCardRemovedFromHand?.Invoke(this, new PlayerCardPayloadEventArgs<CardPayload>(playerId, cardNetworkContainer.cardPayload));
    }
    #endregion

    #region PlayerUI Card Drag
    public void InvokeOnStartHandCardDrag(HandCardDragEventArgs args) {
        OnStartHandCardDrag?.Invoke(this, args);
    }

    public void InvokeOnReleaseHandCardDrag(HandCardDragEventArgs args) {
        OnReleaseHandCardDrag?.Invoke(this, args);
    }

    public void InvokeOnPlayHandCard(PlayerCardUuidEventArgs args) {
        OnPlayHandCard?.Invoke(this, args);
    }
    #endregion

    #region PlayingField Card Drag
    public void InvokeOnStartCardDragPlayingField(PlayingFieldCardEventArgs<CreatureFieldCardUI> args) {
        OnStartCardDragPlayingField?.Invoke(this, args);
    }

    public void InvokeOnReleaseCardDragPlayingField(PlayingFieldCardEventArgs<CreatureFieldCardUI> args) {
        OnReleaseCardDragPlayingField?.Invoke(this, args);
    }

    public void InvokeOnReleaseCreatureFieldCardOverCombatArea(CombatFieldCardEventArgs<CreatureFieldCardUI> args) {
        OnReleaseCreatureFieldCardOverCombatArea?.Invoke(this, args);
    }

    public void InvokeOnSelectAttackerToDefend(SelectAttackerToDefendEventArgs args) {
        OnSelectAttackerToDefend?.Invoke(this, args);
    }
    #endregion

    #region Playing Cards
    public void InvokeOnCreatureCardSelectedForPlay(PlayerCardEventArgs<CreatureCard> args) {
        if (!IsServer)
            return;

        OnCreatureCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnDomainCardSelectedForPlay(PlayerCardEventArgs<DomainCard> args) {
        if (!IsServer)
            return;

        OnDomainCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnSpellCardSelectedForPlay(PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            return;

        OnSpellCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnCreatureCardPlayedFromHand(ulong playerId, CreatureCard card) {
        if (!IsServer)
            return;

        InvokeOnCreatureCardPlayedFromHandClientRpc(playerId, new CreatureCardPayload(card));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnCreatureCardPlayedFromHandClientRpc(ulong playerId, CreatureCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<CreatureCardPayload> args = new PlayerCardPayloadEventArgs<CreatureCardPayload>(playerId, cardPayload);
        OnCreatureCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnDomainCardPlayedFromHand(ulong playerId, DomainCard card) {
        if (!IsServer)
            return;

        InvokeOnDomainCardPlayedFromHandClientRpc(playerId, new DomainCardPayload(card));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDomainCardPlayedFromHandClientRpc(ulong playerId, DomainCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<DomainCardPayload> args = new PlayerCardPayloadEventArgs<DomainCardPayload>(playerId, cardPayload);
        OnDomainCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnSpellCardPlayedFromHand(ulong playerId, SpellCard card) {
        if (!IsServer)
            return;

        InvokeOnSpellCardPlayedFromHandClientRpc(playerId, new SpellCardPayload(card));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellCardPlayedFromHandClientRpc(ulong playerId, SpellCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<SpellCardPayload> args = new PlayerCardPayloadEventArgs<SpellCardPayload>(playerId, cardPayload);
        OnSpellCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnSpellChainCardPlayed(PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            return;

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

    #region Declaring and Undeclaring Creatures
    public void InvokeOnDeclareAttacker(DeclareAttackerEventArgs args) {
        OnDeclareAttacker?.Invoke(this, args);
    }

    public void InvokeOnDeclareDefender(DeclareDefenderEventArgs args) {
        OnDeclareDefender?.Invoke(this, args);
    }

    public void InvokeOnUndelcareAttacker(UndeclareAttackerEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnUnDeclareAttacker can only be called by the server");

        OnUndeclareAttacker?.Invoke(this, args);
    }

    public void InvokeOnUndelcareAttackerFinished(UndeclareAttackerEventArgs args) {
        OnUndeclareAttackerFinished?.Invoke(this, args);
    }

    public void InvokeOnUndeclareDefender(UndeclareDefenderEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnUnDeclareDefender can only be called by the server");

        OnUndeclareDefender?.Invoke(this, args);
    }

    public void InvokeOnUndeclareDefenderFinished(UndeclareDefenderEventArgs args) {
        OnUndeclareDefenderFinished?.Invoke(this, args);
    }
    #endregion

    #region Combat
    public void InvokeOnCreatureAttack(CreatureAttackEventArgs args) {
        OnCreatureAttack?.Invoke(this, args);
    }

    public void InvokeOnCreatureHealed(ulong playerId, CreatureCard card) {
        if (!IsServer)
            throw new Exception("The event OnCreatureHealed can only be invoked by the server");

        InvokeOnCreatureHealedClientRpc(playerId, card);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnCreatureHealedClientRpc(ulong playerId, CreatureCard card) {
        PlayerCardEventArgs<CreatureCard> args = new PlayerCardEventArgs<CreatureCard>(playerId, card);
        OnCreatureHealed?.Invoke(this, args);
    }

    public void InvokeOnCreatureDamagedByCreature(CreatureDamagedByCreatureEventArgs args) {
        OnCreatureDamagedByCreature?.Invoke(this, args);
    }

    public void InvokeOnCreatureDamaged(ulong playerId, CreatureCard card) {
        if (!IsServer)
            throw new Exception("The event OnCreatureDamaged can only be invoked by the server");

        InvokeOnCreatureDamagedClientRpc(playerId, card);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnCreatureDamagedClientRpc(ulong playerId, CreatureCard card) {
        PlayerCardEventArgs<CreatureCard> args = new PlayerCardEventArgs<CreatureCard>(playerId, card);
        OnCreatureDamaged?.Invoke(this, args);
    }

    public void InvokeOnCreatureDestroyed(ulong playerId, CreatureCard card) {
        if (!IsServer)
            throw new Exception("The event OnCreatureDestroyed can only be invoked by the server");

        InvokeOnCreatureDestroyedClientRpc(playerId, card);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnCreatureDestroyedClientRpc(ulong playerId, CreatureCard card) {
        PlayerCardEventArgs<CreatureCard> args = new PlayerCardEventArgs<CreatureCard>(playerId, card);
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
