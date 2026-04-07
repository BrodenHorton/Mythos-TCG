using System;

// TODO: Add regions for the invoke methods that match the event categories
public static class EventBus {
    // Player Status Changes
    public static event EventHandler<LifePointsChangedEventArgs> OnLifePointsChanged;
    public static event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    // Duelist UI Actions
    public static event EventHandler<PlayerCardEventArgs> OnCardDrawn;
    public static event EventHandler<CardRemovedFromHandEventArgs> OnCardRemovedFromHand;
    // PlayerUI card drag
    public static event EventHandler<HandCardDragEventArgs> OnStartHandCardDrag;
    public static event EventHandler<HandCardDragEventArgs> OnReleaseHandCardDrag;
    public static event EventHandler<HandCardEnteringPlayingFieldEventArgs> OnHandCardEnteringPlayingField;
    // PlayingField card drag
    public static event EventHandler<CreatureFieldCardDragEventArgs> OnStartCardDragPlayingField;
    public static event EventHandler<ReleaseCreatureFieldCardDragEventArgs> OnReleaseCardDragPlayingField;
    public static event EventHandler<CreatureFieldCardEnteringCombatFieldEventArgs> OnReleaseCreatureFieldCardOverCombatArea;
    // Playing Cards
    public static event EventHandler<PlayCreatureCardFromHandEventArgs> OnCreatureCardSelectedForPlay;
    public static event EventHandler<PlayCreatureCardFromHandEventArgs> OnCreatureCardPlayedFromHand;
    public static event EventHandler<PlayerSpellCardEventArgs> OnSpellCardPlayed;
    public static event EventHandler<PlayerSpellCardEventArgs> OnDomainCardPlayed;
    // Declaring and Undeclaring creatures
    public static event EventHandler<DeclareAttackerEventArgs> OnDeclareAttacker;
    public static event EventHandler<DeclareDefenderEventArgs> OnDeclareDefender;
    public static event EventHandler<UndeclareAttackerEventArgs> OnUndeclareAttacker;
    public static event EventHandler<UndeclareDefenderEventArgs> OnUndeclareDefender;
    public static event EventHandler<SelectAttackerToDefendEventArgs> OnSelectAttackerToDefend;
    // Combat
    public static event EventHandler<AttackEventArgs> OnAttack;
    public static event EventHandler<PlayerCreatureCardEventArgs> OnCreatureDestroyed;
    public static event EventHandler<ReleaseCombatCreaturesEventArgs> OnReleaseCombatCreatures;
    // Creature Actions
    public static event EventHandler<CreatureCardEventArgs> OnCreatureTapped;
    public static event EventHandler<CreatureCardEventArgs> OnCreatureUntapped;
    // Player Actions
    public static event EventHandler OnActionButtonPressed;
    public static event EventHandler OnLocalClientPlayerReadyUp;

    public static void InvokeOnLifePointsChanged(object sender, LifePointsChangedEventArgs args) {
        OnLifePointsChanged?.Invoke(sender, args);
    }

    public static void InvokeOnManaCountChanged(object sender, ManaChangedEventArgs args) {
        OnManaCountChanged?.Invoke(sender, args);
    }

    public static void InvokeOnCardDrawn(object sender, PlayerCardEventArgs args) {
        OnCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnCardRemovedFromHand(object sender, CardRemovedFromHandEventArgs args) {
        OnCardRemovedFromHand?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureCardSelectedForPlay(object sender, PlayCreatureCardFromHandEventArgs args) {
        OnCreatureCardSelectedForPlay?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureCardPlayedFromHand(object sender, PlayCreatureCardFromHandEventArgs args) {
        OnCreatureCardPlayedFromHand?.Invoke(sender, args);
    }

    public static void InvokeOnSpellCardPlayed(object sender, PlayerSpellCardEventArgs args) {
        OnSpellCardPlayed?.Invoke(sender, args);
    }

    public static void InvokeOnDomainCardPlayed(object sender, PlayerSpellCardEventArgs args) {
        OnDomainCardPlayed?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureTapped(object sender, CreatureCardEventArgs args) {
        OnCreatureTapped?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureUntapped(object sender, CreatureCardEventArgs args) {
        OnCreatureUntapped?.Invoke(sender, args);
    }

    public static void InvokeOnDeclareAttacker(object sender, DeclareAttackerEventArgs args) {
        OnDeclareAttacker?.Invoke(sender, args);
    }

    public static void InvokeOnDeclareDefender(object sender, DeclareDefenderEventArgs args) {
        OnDeclareDefender?.Invoke(sender, args);
    }

    public static void InvokeOnUndelcareAttacker(object sender, UndeclareAttackerEventArgs args) {
        OnUndeclareAttacker?.Invoke(sender, args);
    }

    public static void InvokeOnUndeclareDefender(object sender, UndeclareDefenderEventArgs args) {
        OnUndeclareDefender?.Invoke(sender, args);
    }

    public static void InvokeOnSelectAttackerToDefend(object sender, SelectAttackerToDefendEventArgs args) {
        OnSelectAttackerToDefend?.Invoke(sender, args);
    }

    public static void InvokeOnAttack(object sender, AttackEventArgs args) {
        OnAttack?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureDestroyed(object sender, PlayerCreatureCardEventArgs args) {
        OnCreatureDestroyed?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCombatCreatures(object sender, ReleaseCombatCreaturesEventArgs args) {
        OnReleaseCombatCreatures?.Invoke(sender, args);
    }

    public static void InvokeOnActionButtonPressed(object sender, EventArgs args) {
        OnActionButtonPressed?.Invoke(sender, args);
    }

    public static void InvokeOnStartHandCardDrag(object sender, HandCardDragEventArgs args) {
        OnStartHandCardDrag?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseHandCardDrag(object sender, HandCardDragEventArgs args) {
        OnReleaseHandCardDrag?.Invoke(sender, args);
    }

    public static void InvokeOnHandCardEnteringPlayingField(object sender, HandCardEnteringPlayingFieldEventArgs args) {
        OnHandCardEnteringPlayingField?.Invoke(sender, args);
    }

    public static void InvokeOnStartCardDragPlayingField(object sender, CreatureFieldCardDragEventArgs args) {
        OnStartCardDragPlayingField?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCardDragPlayingField(object sender, ReleaseCreatureFieldCardDragEventArgs args) {
        OnReleaseCardDragPlayingField?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCreatureFieldCardOverCombatArea(object sender, CreatureFieldCardEnteringCombatFieldEventArgs args) {
        OnReleaseCreatureFieldCardOverCombatArea?.Invoke(sender, args);
    }

    public static void InvokeOnLocalClientPlayerReadyUp(object sender, EventArgs args) {
        OnLocalClientPlayerReadyUp?.Invoke(sender, args);
    }
}
