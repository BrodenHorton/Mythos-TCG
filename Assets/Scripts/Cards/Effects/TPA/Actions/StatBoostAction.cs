using System;

public class StatBoostAction : EffectAction<StatBoostEffectContext, EventArgs> {
    public void Execute(StatBoostEffectContext effect, EventArgs args) {
        effect.effectProkCount++;
    }
}
