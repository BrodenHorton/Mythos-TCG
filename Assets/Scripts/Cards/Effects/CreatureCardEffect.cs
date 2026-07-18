using System;

[Serializable]
public abstract class CreatureCardEffect {
    protected CreatureCard card;
    protected string effectName;
    protected string description;

    public CreatureCardEffect() { }

    public abstract void Init(CreatureCard card);

    public abstract void RemoveListeners();

    public abstract CreatureCardEffect DeepCopy();

    public abstract string GetFullDescription();

    public abstract CreatureCardEffectPayload GetEffectPayload();

    public CreatureCard Card { get { return card; } }

    public string EffectName { get { return effectName; } }
}
