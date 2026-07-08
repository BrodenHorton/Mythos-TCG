using System;

[Serializable]
public abstract class CreatureCardEffect {
    protected CreatureCard card;
    protected string effectName;
    protected string description;

    public CreatureCardEffect() { }

    public abstract void Init(CreatureCard card);

    public abstract void RemoveListeners();

    public abstract bool IsStaticKeyword();

    public abstract CreatureCardEffect DeepCopy();

    public abstract CreatureCardEffectPayload GetEffectPayload();

    public CreatureCard Card { get { return card; } }

    public string EffectName { get { return effectName; } }

    public string Description { get { return description; } }
}
