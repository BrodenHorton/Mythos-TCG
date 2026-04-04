using System;

public static class EventBus {
    public static event EventHandler<LifePointsChangedEventArgs> OnLifePointsChanged;
    public static event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    public static event EventHandler<PlayerCardEventArgs> OnCardDrawn;
    public static event EventHandler<CardRemovedFromHandEventArgs> OnCardRemovedFromHand;
    public static event EventHandler<PlayCreatureCardFromHandEventArgs> OnCreatureCardSelectedForPlay;
    public static event EventHandler<PlayCreatureCardFromHandEventArgs> OnCreatureCardPlayedFromHand;
    public static event EventHandler<PlayerSpellCardEventArgs> OnSpellCardPlayed;
    public static event EventHandler<PlayerSpellCardEventArgs> OnDomainCardPlayed;
    public static event EventHandler<CreatureCardEventArgs> OnCreatureTapped;
    public static event EventHandler<CreatureCardEventArgs> OnCreatureUntapped;
    public static event EventHandler<DeclareAttackerEventArgs> OnDeclareAttacker;
    public static event EventHandler<DeclareDefenderEventArgs> OnDeclareDefender;
    public static event EventHandler<UndeclareAttackerEventArgs> OnUndeclareAttacker;
    public static event EventHandler<UndeclareDefenderEventArgs> OnUndeclareDefender;
    public static event EventHandler<AttackEventArgs> OnAttack;
    public static event EventHandler<ReleaseCombatCreaturesEventArgs> OnReleaseCombatCreatures;
    public static event EventHandler OnActionButtonPressed;
    public static event EventHandler<PlayingFieldCreatureCardDragEventArgs> OnStartCardDragPlayingField;
    public static event EventHandler<ReleaseCreatureFieldCardDragEventArgs> OnReleaseCardDragPlayingField;
    public static event EventHandler<CreatureFieldCardEnteringCombatAreaEventArgs> OnReleaseCreatureFieldCardOverCombatArea;
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

    public static void InvokeOnDelcareAttacker(object sender, DeclareAttackerEventArgs args) {
        OnDeclareAttacker?.Invoke(sender, args);
    }

    public static void InvokeOnDelcareDefender(object sender, DeclareDefenderEventArgs args) {
        OnDeclareDefender?.Invoke(sender, args);
    }

    public static void InvokeOnUndelcareAttacker(object sender, UndeclareAttackerEventArgs args) {
        OnUndeclareAttacker?.Invoke(sender, args);
    }

    public static void InvokeOnUndeclareDefender(object sender, UndeclareDefenderEventArgs args) {
        OnUndeclareDefender?.Invoke(sender, args);
    }

    public static void InvokeOnAttack(object sender, AttackEventArgs args) {
        OnAttack?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCombatCreatures(object sender, ReleaseCombatCreaturesEventArgs args) {
        OnReleaseCombatCreatures?.Invoke(sender, args);
    }

    public static void InvokeOnActionButtonPressed(object sender, EventArgs args) {
        TcgLogger.Log("ActionButtonPressed Invoked Entered");
        OnActionButtonPressed?.Invoke(sender, args);
    }

    public static void InvokeOnStartCardDragPlayingField(object sender, PlayingFieldCreatureCardDragEventArgs args) {
        OnStartCardDragPlayingField?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCardDragPlayingField(object sender, ReleaseCreatureFieldCardDragEventArgs args) {
        OnReleaseCardDragPlayingField?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCreatureFieldCardOverCombatArea(object sender, CreatureFieldCardEnteringCombatAreaEventArgs args) {
        OnReleaseCreatureFieldCardOverCombatArea?.Invoke(sender, args);
    }

    public static void InvokeOnLocalClientPlayerReadyUp(object sender, EventArgs args) {
        TcgLogger.Log("InvokeOnLocalClientPlayerReadyUp Entered");
        OnLocalClientPlayerReadyUp?.Invoke(sender, args);
    }
}
