using System;

public class FloatEventArgs : EventArgs {
    private float value;

    public FloatEventArgs(float value) {
        this.value = value;
    }

    public float Value { get { return value; } }
}
