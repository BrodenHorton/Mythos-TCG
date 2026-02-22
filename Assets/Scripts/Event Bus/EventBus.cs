using System;

public static class EventBus {
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
}