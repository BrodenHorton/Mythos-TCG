using System;

[Serializable]
public abstract class CreatureCardEffect {
    protected CreatureCard card;

    public CreatureCardEffect() { }

    public abstract void Init(CreatureCard card);

    public abstract void RemoveListeners();

    public abstract CreatureCardEffect DeepCopy();

    public abstract CreatureCardEffectPayload GetEffectPayload();

    public CreatureCard Card { get { return card; } }
}
