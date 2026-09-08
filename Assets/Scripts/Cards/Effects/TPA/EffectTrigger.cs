using System;

public abstract class EffectTrigger<T> where T : EventArgs {
    public event EventHandler<T> OnTriggerEffect;

    public abstract void Init();

    public abstract void RemoveListeners();

    protected void InvokeOnTriggerEffect(object sender, T args) {
        OnTriggerEffect?.Invoke(sender, args);
    }
}