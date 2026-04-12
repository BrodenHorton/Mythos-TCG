using System;

public static class EventBus {
    // Duelist UI Actions
    public static event EventHandler<PlayerCardEventArgs<Card>> OnCardDrawn;
    public static event EventHandler<CardRemovedFromHandEventArgs> OnCardRemovedFromHand;
    // PlayerUI Card Drag
    public static event EventHandler<HandCardDragEventArgs> OnStartHandCardDrag;
    public static event EventHandler<HandCardDragEventArgs> OnReleaseHandCardDrag;
    public static event EventHandler<HandCardEnteringPlayingFieldEventArgs> OnHandCardEnteringPlayingField;
    // PlayingField Card Drag
    public static event EventHandler<FieldCardDragEventArgs<CreatureFieldCardUI>> OnStartCardDragPlayingField;
    public static event EventHandler<ReleaseFieldCardDragEventArgs<CreatureFieldCardUI>> OnReleaseCardDragPlayingField;
    public static event EventHandler<CreatureFieldCardEnteringCombatFieldEventArgs> OnReleaseCreatureFieldCardOverCombatArea;
    // Playing Cards
    public static event EventHandler<PlayCardFromHandEventArgs<CreatureCard>> OnCreatureCardSelectedForPlay;
    public static event EventHandler<PlayCardFromHandEventArgs<CreatureCard>> OnCreatureCardPlayedFromHand;
    public static event EventHandler<PlayCardFromHandEventArgs<SpellCard>> OnSpellCardSelectedForPlay;
    public static event EventHandler<PlayCardFromHandEventArgs<SpellCard>> OnSpellCardPlayedFromHand;
    public static event EventHandler<PlayCardFromHandEventArgs<DomainCard>> OnDomainCardSelectedForPlay;
    public static event EventHandler<PlayCardFromHandEventArgs<DomainCard>> OnDomainCardPlayedFromHand;
    // Player Status Changes
    public static event EventHandler<LifePointsChangedEventArgs> OnLifePointsChanged;
    public static event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    // Creature Status Changes
    public static event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureHealthChanged;
    // Declaring and Undeclaring creatures
    public static event EventHandler<DeclareAttackerEventArgs> OnDeclareAttacker;
    public static event EventHandler<DeclareDefenderEventArgs> OnDeclareDefender;
    public static event EventHandler<UndeclareAttackerEventArgs> OnUndeclareAttacker;
    public static event EventHandler<UndeclareDefenderEventArgs> OnUndeclareDefender;
    public static event EventHandler<SelectAttackerToDefendEventArgs> OnSelectAttackerToDefend;
    // Combat
    public static event EventHandler<AttackEventArgs> OnAttack;
    public static event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureDamaged;
    public static event EventHandler<PlayerCardEventArgs<CreatureCard>> OnCreatureDestroyed;
    public static event EventHandler<ReleaseCombatCreaturesEventArgs> OnReleaseCombatCreatures;
    // Creature Actions
    public static event EventHandler<CreatureCardEventArgs> OnCreatureTapped;
    public static event EventHandler<CreatureCardEventArgs> OnCreatureUntapped;
    // Player Actions
    public static event EventHandler OnActionButtonPressed;
    public static event EventHandler OnLocalClientPlayerReadyUp;

    #region Duelist UI Actions
    public static void InvokeOnCardDrawn(object sender, PlayerCardEventArgs<Card> args) {
        OnCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnCardRemovedFromHand(object sender, CardRemovedFromHandEventArgs args) {
        OnCardRemovedFromHand?.Invoke(sender, args);
    }
    #endregion

    #region PlayerUI Card Drag
    public static void InvokeOnStartHandCardDrag(object sender, HandCardDragEventArgs args) {
        OnStartHandCardDrag?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseHandCardDrag(object sender, HandCardDragEventArgs args) {
        OnReleaseHandCardDrag?.Invoke(sender, args);
    }

    public static void InvokeOnHandCardEnteringPlayingField(object sender, HandCardEnteringPlayingFieldEventArgs args) {
        OnHandCardEnteringPlayingField?.Invoke(sender, args);
    }
    #endregion

    #region PlayingField Card Drag
    public static void InvokeOnStartCardDragPlayingField(object sender, FieldCardDragEventArgs<CreatureFieldCardUI> args) {
        OnStartCardDragPlayingField?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCardDragPlayingField(object sender, ReleaseFieldCardDragEventArgs<CreatureFieldCardUI> args) {
        OnReleaseCardDragPlayingField?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCreatureFieldCardOverCombatArea(object sender, CreatureFieldCardEnteringCombatFieldEventArgs args) {
        OnReleaseCreatureFieldCardOverCombatArea?.Invoke(sender, args);
    }
    #endregion

    #region Playing Cards
    public static void InvokeOnCreatureCardSelectedForPlay(object sender, PlayCardFromHandEventArgs<CreatureCard> args) {
        OnCreatureCardSelectedForPlay?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureCardPlayedFromHand(object sender, PlayCardFromHandEventArgs<CreatureCard> args) {
        OnCreatureCardPlayedFromHand?.Invoke(sender, args);
    }

    public static void InvokeOnSpellCardSelectedForPlay(object sender, PlayCardFromHandEventArgs<SpellCard> args) {
        OnSpellCardSelectedForPlay?.Invoke(sender, args);
    }

    public static void InvokeOnSpellCardPlayedFromHand(object sender, PlayCardFromHandEventArgs<SpellCard> args) {
        OnSpellCardPlayedFromHand?.Invoke(sender, args);
    }

    public static void InvokeOnDomainCardSelectedForPlay(object sender, PlayCardFromHandEventArgs<DomainCard> args) {
        OnDomainCardSelectedForPlay?.Invoke(sender, args);
    }

    public static void InvokeOnDomainCardPlayedFromHand(object sender, PlayCardFromHandEventArgs<DomainCard> args) {
        OnDomainCardPlayedFromHand?.Invoke(sender, args);
    }
    #endregion

    #region Player Status Changes
    public static void InvokeOnLifePointsChanged(object sender, LifePointsChangedEventArgs args) {
        OnLifePointsChanged?.Invoke(sender, args);
    }

    public static void InvokeOnManaCountChanged(object sender, ManaChangedEventArgs args) {
        OnManaCountChanged?.Invoke(sender, args);
    }
    #endregion

    #region Creature Status Changes
    public static void InvokeOnCreatureHealthChanged(object sender, PlayerCardEventArgs<CreatureCard> args) {
        OnCreatureHealthChanged?.Invoke(sender, args);
    }
    #endregion

    #region Declaring and Undeclaring Creatures
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
    #endregion

    #region Combat
    public static void InvokeOnAttack(object sender, AttackEventArgs args) {
        OnAttack?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureDamaged(object sender, PlayerCardEventArgs<CreatureCard> args) {
        OnCreatureDamaged?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureDestroyed(object sender, PlayerCardEventArgs<CreatureCard> args) {
        OnCreatureDestroyed?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCombatCreatures(object sender, ReleaseCombatCreaturesEventArgs args) {
        OnReleaseCombatCreatures?.Invoke(sender, args);
    }
    #endregion

    #region Creature Actions
    public static void InvokeOnCreatureTapped(object sender, CreatureCardEventArgs args) {
        OnCreatureTapped?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureUntapped(object sender, CreatureCardEventArgs args) {
        OnCreatureUntapped?.Invoke(sender, args);
    }
    #endregion

    #region Player Actions
    public static void InvokeOnActionButtonPressed(object sender, EventArgs args) {
        OnActionButtonPressed?.Invoke(sender, args);
    }

    public static void InvokeOnLocalClientPlayerReadyUp(object sender, EventArgs args) {
        OnLocalClientPlayerReadyUp?.Invoke(sender, args);
    }
    #endregion
}
