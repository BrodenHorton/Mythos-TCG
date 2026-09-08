using System;

public class StatBoostEffectContext : EffectContext<CreatureCard> {
    public int effectProkCount;

    public StatBoostEffectContext(string id, string effectName) : base(id, effectName) {
        effectProkCount = 0;
    }

    public override void Init(CreatureCard card) {
        throw new NotImplementedException();
    }

    public override void RemoveListeners() {
        throw new NotImplementedException();
    }
}
