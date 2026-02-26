using System;

public static class EventBus {
    public static event EventHandler<LifePointsChangedEventArgs> OnLifePointsChanged;
    public static event EventHandler<ManaChangedEventArgs> OnManaCountChanged;
    public static event EventHandler<PlayerCardEventArgs> OnCardDrawn;
    public static event EventHandler<PlayerCreatureCardEventArgs> OnCreatureCardDrawn;
    public static event EventHandler<PlayerSpellCardEventArgs> OnSpellCardDrawn;
    public static event EventHandler<PlayerCreatureCardEventArgs> OnCreatureCardPlayed;
    public static event EventHandler<PlayerSpellCardEventArgs> OnSpellCardPlayed;
    public static event EventHandler<PlayerSpellCardEventArgs> OnDomainCardPlayed;
    public static event EventHandler<CreatureCardEventArgs> OnCreatureTapped;
    public static event EventHandler<CreatureCardEventArgs> OnCreatureUntapped;
    public static event EventHandler<DeclareAttackerEventArgs> OnDeclareAttacker;
    public static event EventHandler<DeclareDefenderEventArgs> OnDeclareDefender;
    public static event EventHandler<UndeclareAttackerEventArgs> OnUndeclareAttacker;
    public static event EventHandler<AttackEventArgs> OnAttack;
    public static event EventHandler<ReleaseCombatCreaturesEventArgs> OnReleaseCombatCreatures;
    public static event EventHandler<EventArgs> OnActionButtonPressed;

    public static void InvokeOnLifePointsChanged(object sender, LifePointsChangedEventArgs args) {
        OnLifePointsChanged?.Invoke(sender, args);
    }

    public static void InvokeOnManaCountChanged(object sender, ManaChangedEventArgs args) {
        OnManaCountChanged?.Invoke(sender, args);
    }

    public static void InvokeOnCardDrawn(object sender, PlayerCardEventArgs args) {
        OnCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureCardDrawn(object sender, PlayerCreatureCardEventArgs args) {
        OnCreatureCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnSpellCardDrawn(object sender, PlayerSpellCardEventArgs args) {
        OnSpellCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureCardPlayed(object sender, PlayerCreatureCardEventArgs args) {
        OnCreatureCardPlayed?.Invoke(sender, args);
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

    public static void InvokeOnAttack(object sender, AttackEventArgs args) {
        OnAttack?.Invoke(sender, args);
    }

    public static void InvokeOnReleaseCombatCreatures(object sender, ReleaseCombatCreaturesEventArgs args) {
        OnReleaseCombatCreatures?.Invoke(sender, args);
    }

    public static void InvokeOnActionButtonPressed(object sender, EventArgs args) {
        OnActionButtonPressed?.Invoke(sender, args);
    }
}