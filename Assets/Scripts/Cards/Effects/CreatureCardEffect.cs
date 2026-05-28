using System;
using Unity.Netcode;

[Serializable]
public abstract class CreatureCardEffect {
    protected Guid creatureCardUuid;

    public CreatureCardEffect() { }

    public abstract void Init(Guid creatureCardUuid);

    public abstract void RemoveListeners();

    public abstract CreatureCardEffect DeepCopy();

    public abstract CreatureCardEffectPayload GetEffectPayload();

    public Guid CreatureCardUuid { get { return creatureCardUuid; } }
}
