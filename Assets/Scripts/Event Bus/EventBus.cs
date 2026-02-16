using System;
using UnityEngine;

public static class EventBus {
    public static event EventHandler<DrawCardEventArgs> OnCreatureCardDrawn;
    public static event EventHandler<DrawCardEventArgs> OnSpellCardDrawn;

    public static void InvokeOnCreatureCardDrawn(object sender, DrawCardEventArgs args) {
        OnCreatureCardDrawn?.Invoke(sender, args);
    }

    public static void InvokeOnSpellCardDrawn(object sender, DrawCardEventArgs args) {
        OnSpellCardDrawn?.Invoke(sender, args);
    }
}
