using System;
using UnityEngine;

public static class EventBus {
    public static event EventHandler<DrawCreatureCardEventArgs> OnCreatureCardDrawn;
    public static event EventHandler<DrawSpellCardEventArgs> OnSpellCardDrawn;
    public static event EventHandler<PlayCreatureCardEventArgs> OnCreatureCardPlayed;
    public static event EventHandler<PlaySpellCardEventArgs> OnSpellCardPlayed;

    public static void InvokeOnCreatureCardDrawn(object sender, DrawCreatureCardEventArgs args) {
        OnCreatureCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnSpellCardDrawn(object sender, DrawSpellCardEventArgs args) {
        OnSpellCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnCreatureCardPlayed(object sender, PlayCreatureCardEventArgs args) {
        OnCreatureCardPlayed?.Invoke(sender, args);
    }

    public static void InvokeOnSpellCardPlayed(object sender, PlaySpellCardEventArgs args) {
        OnSpellCardPlayed?.Invoke(sender, args);
    }
}
