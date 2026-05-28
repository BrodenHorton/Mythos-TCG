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
    public event EventHandler<SelectAttackerToDefendUIEventArgs> OnSelectAttackerToDefendUI;
    // Playing Cards
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureCardSelectedForPlay;
    public event EventHandler<PlayerCardEventArgs<DomainCard>> OnDomainCardSelectedForPlay;
    public event EventHandler<PlayerCardEventArgs<SpellCard>> OnSpellCardSelectedForPlay;
    public event EventHandler<PlayerCardPayloadEventArgs<CreatureCardPayload>> OnCreatureCardPlayedFromHand;
    public event EventHandler<PlayerCardPayloadEventArgs<DomainCardPayload>> OnDomainCardPlayedFromHand;
    public event EventHandler<PlayerCardEventArgs<SpellCard>> OnSpellCardPlayedFromHand;
    public event EventHandler<PlayerCardEventArgs<SpellCard>> OnSpellChainCardPlayed;
    // Player Status Changes
    public event EventHandler<LifePointsChangedEventArgs> OnLifePointsChanged;
    public event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    // Declaring and Undeclaring creatures
    public event EventHandler<PlayerCardCancelableEventArgs<CreatureCard>> OnCanCreatureAttack;
    public event EventHandler<PlayerCardCancelableEventArgs<CreatureCard>> OnCanCreatureDefend;
    public event EventHandler<CanDefendEventArgs> OnSelectAttackerToDefend;
    public event EventHandler<CombatCreatureEventArgs> OnDeclareAttacker;
    public event EventHandler<CombatCreaturePayloadEventArgs> OnDeclareAttackerFinished;
    public event EventHandler<CreatureCombatEventArgs> OnDeclareDefender;
    public event EventHandler<CreatureCombatPayloadEventArgs> OnDeclareDefenderFinished;
    public event EventHandler<CombatCreatureEventArgs> OnUndeclareAttacker;
    public event EventHandler<CombatCreaturePayloadEventArgs> OnUndeclareAttackerFinished;
    public event EventHandler<CombatCreatureEventArgs> OnUndeclareDefender;
    public event EventHandler<CombatCreaturePayloadEventArgs> OnUndeclareDefenderFinished;
    // Combat
    public event EventHandler<CreatureCombatEventArgs> OnCreatureAttack;
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureHealed;
    public event EventHandler<PlayerCardPayloadEventArgs<CreatureCardPayload>> OnCreatureHealedFinished;
    public event EventHandler<CreatureDamagedByCreatureEventArgs> OnCreatureDamagedByCreature;
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureDamaged;
    public event EventHandler<PlayerCardPayloadEventArgs<CreatureCardPayload>> OnCreatureDamagedFinished;
    public event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureDestroyed;
    public event EventHandler<PlayerCardPayloadEventArgs<CreatureCardPayload>> OnCreatureDestroyedFinished;
    public event EventHandler<ReleaseCombatCreaturesEventArgs> OnReleaseCombatCreatures;
    // Creature Actions
    public event EventHandler<PlayerCardCancelableEventArgs<CreatureCard>> OnEnteringFieldSummoningSickness;
    public event EventHandler<PlayerCardCancelableEventArgs<CreatureCard>> OnCreatureTapped;
    public event EventHandler<PlayerCardPayloadEventArgs<CreatureCardPayload>> OnCreatureTappedFinished;
    public event EventHandler<PlayerCardCancelableEventArgs<CreatureCard>> OnCreatureUntapped;
    public event EventHandler<PlayerCardPayloadEventArgs<CreatureCardPayload>> OnCreatureUntappedFinished;
    // Creature Effects
    public event EventHandler<CanDefendEventArgs> OnSelectElusiveAttackerToDefend;

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

    public void InvokeOnSelectAttackerToDefendUI(SelectAttackerToDefendUIEventArgs args) {
        OnSelectAttackerToDefendUI?.Invoke(this, args);
    }
    #endregion

    #region Playing Cards
    public void InvokeOnCreatureCardSelectedForPlay(PlayerCardEventArgs<CreatureCard> args) {
        if (!IsServer)
            throw new Exception("The event OnCreatureCardSelectedForPlay can only be invoked by the server");

        OnCreatureCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnDomainCardSelectedForPlay(PlayerCardEventArgs<DomainCard> args) {
        if (!IsServer)
            throw new Exception("The event OnDomainCardSelectedForPlay can only be invoked by the server");

        OnDomainCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnSpellCardSelectedForPlay(PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            throw new Exception("The event OnSpellCardSelectedForPlay can only be invoked by the server");

        OnSpellCardSelectedForPlay?.Invoke(this, args);
    }

    public void InvokeOnCreatureCardPlayedFromHand(ulong playerId, CreatureCard card) {
        if (!IsServer)
            throw new Exception("The event OnCreatureCardPlayedFromHand can only be invoked by the server");

        InvokeOnCreatureCardPlayedFromHandClientRpc(playerId, new CreatureCardPayload(card));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnCreatureCardPlayedFromHandClientRpc(ulong playerId, CreatureCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<CreatureCardPayload> args = new PlayerCardPayloadEventArgs<CreatureCardPayload>(playerId, cardPayload);
        OnCreatureCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnDomainCardPlayedFromHand(ulong playerId, DomainCard card) {
        if (!IsServer)
            throw new Exception("The event OnDomainCardPlayedFromHand can only be invoked by the server");

        InvokeOnDomainCardPlayedFromHandClientRpc(playerId, new DomainCardPayload(card));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnDomainCardPlayedFromHandClientRpc(ulong playerId, DomainCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<DomainCardPayload> args = new PlayerCardPayloadEventArgs<DomainCardPayload>(playerId, cardPayload);
        OnDomainCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnSpellCardPlayedFromHand(PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            throw new Exception("The event OnSpellCardPlayedFromHand can only be invoked by the server");

        OnSpellCardPlayedFromHand?.Invoke(this, args);
    }

    public void InvokeOnSpellChainCardPlayed(PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            throw new Exception("The event OnSpellChainCardPlayed can only be invoked by the server");

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
    public void InvokeOnCanCreatureAttack(PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (!IsServer)
            throw new Exception("The event OnCanCreatureAttack can only be called by the server");

        OnCanCreatureAttack?.Invoke(this, args);
    }

    public void InvokeOnCanCreatureDefend(PlayerCardCancelableEventArgs<CreatureCard> args) {
        if (!IsServer)
            throw new Exception("The event OnCanCreatureDefend can only be called by the server");

        OnCanCreatureDefend?.Invoke(this, args);
    }

    public void InvokeOnSelectAttackerToDefend(CanDefendEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnSelectAttackerToDefend can only be called by the server");

        OnSelectAttackerToDefend?.Invoke(this, args);
    }

    public void InvokeOnDeclareAttacker(CombatCreatureEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnDeclareAttacker can only be called by the server");

        OnDeclareAttacker?.Invoke(this, args);
    }

    public void InvokeOnDeclareAttackerFinished(CombatCreaturePayloadEventArgs args) {
        OnDeclareAttackerFinished?.Invoke(this, args);
    }

    public void InvokeOnDeclareDefender(CreatureCombatEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnDeclareDefender can only be called by the server");

        OnDeclareDefender?.Invoke(this, args);
    }

    public void InvokeOnDeclareDefenderFinished(CreatureCombatPayloadEventArgs args) {
        OnDeclareDefenderFinished?.Invoke(this, args);
    }

    public void InvokeOnUndelcareAttacker(CombatCreatureEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnUnDeclareAttacker can only be called by the server");

        OnUndeclareAttacker?.Invoke(this, args);
    }

    public void InvokeOnUndelcareAttackerFinished(CombatCreaturePayloadEventArgs args) {
        OnUndeclareAttackerFinished?.Invoke(this, args);
    }

    public void InvokeOnUndeclareDefender(CombatCreatureEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnUnDeclareDefender can only be called by the server");

        OnUndeclareDefender?.Invoke(this, args);
    }

    public void InvokeOnUndeclareDefenderFinished(CombatCreaturePayloadEventArgs args) {
        OnUndeclareDefenderFinished?.Invoke(this, args);
    }
    #endregion

    #region Combat
    public void InvokeOnCreatureAttack(CreatureCombatEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnCreatureAttack can only be invoked by the server");

        OnCreatureAttack?.Invoke(this, args);
    }

    public void InvokeOnCreatureHealed(PlayerCardEventArgs<CreatureCard> args) {
        if (!IsServer)
            throw new Exception("The event OnCreatureHealed can only be invoked by the server");

        OnCreatureHealed?.Invoke(this, args);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InvokeOnCreatureHealedFinishedClientRpc(ulong playerId, CreatureCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<CreatureCardPayload> args = new PlayerCardPayloadEventArgs<CreatureCardPayload>(playerId, cardPayload);
        OnCreatureHealedFinished?.Invoke(this, args);
    }

    public void InvokeOnCreatureDamagedByCreature(CreatureDamagedByCreatureEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnCreatureDamagedByCreature can only be invoked by the server");

        OnCreatureDamagedByCreature?.Invoke(this, args);
    }

    public void InvokeOnCreatureDamaged(PlayerCardEventArgs<CreatureCard> args) {
        if (!IsServer)
            throw new Exception("The event OnCreatureDamaged can only be invoked by the server");

        OnCreatureDamaged?.Invoke(this, args);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InvokeOnCreatureDamagedFinishedClientRpc(ulong playerId, CreatureCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<CreatureCardPayload> args = new PlayerCardPayloadEventArgs<CreatureCardPayload>(playerId, cardPayload);
        OnCreatureDamagedFinished?.Invoke(this, args);
    }

    public void InvokeOnCreatureDestroyed(PlayerCardEventArgs<CreatureCard> args) {
        if (!IsServer)
            throw new Exception("The event OnCreatureDestroyed can only be invoked by the server");

        OnCreatureDestroyed?.Invoke(this, args);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InvokeOnCreatureDestroyedFinishedClientRpc(ulong playerId, CreatureCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<CreatureCardPayload> args = new PlayerCardPayloadEventArgs<CreatureCardPayload>(playerId, cardPayload);
        OnCreatureDestroyedFinished?.Invoke(this, args);
    }

    public void InvokeOnReleaseCombatCreatures(ReleaseCombatCreaturesEventArgs args) {
        OnReleaseCombatCreatures?.Invoke(this, args);
    }
    #endregion

    #region Creature Actions
    public void InvokeOnEnteringFieldSummoningSickness(PlayerCardCancelableEventArgs<CreatureCard> args) {
        OnEnteringFieldSummoningSickness?.Invoke(this, args);
    }

    public void InvokeOnCreatureTapped(PlayerCardCancelableEventArgs<CreatureCard> args) {
        OnCreatureTapped?.Invoke(this, args);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InvokeOnCreatureTappedFinishedClientRpc(ulong playerId, CreatureCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<CreatureCardPayload> args = new PlayerCardPayloadEventArgs<CreatureCardPayload>(playerId, cardPayload);
        OnCreatureTappedFinished?.Invoke(this, args);
    }

    public void InvokeOnCreatureUntapped(PlayerCardCancelableEventArgs<CreatureCard> args) {
        OnCreatureUntapped?.Invoke(this, args);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void InvokeOnCreatureUntappedFinishedClientRpc(ulong playerId, CreatureCardPayload cardPayload) {
        PlayerCardPayloadEventArgs<CreatureCardPayload> args = new PlayerCardPayloadEventArgs<CreatureCardPayload>(playerId, cardPayload);
        OnCreatureUntappedFinished?.Invoke(this, args);
    }
    #endregion

    #region Creature Effects
    public void InvokeOnSelectElusiveAttackerToDefend(CanDefendEventArgs args) {
        if (!IsServer)
            throw new Exception("The event OnSelectElusiveAttackerToDefend can only be called by the server");

        OnSelectElusiveAttackerToDefend?.Invoke(this, args);
    }
    #endregion
}
