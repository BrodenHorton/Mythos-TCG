using Unity.Netcode;

public struct CreatureCombatNetworkContainer : INetworkSerializable {
    public ulong initiatorId;
    public ulong targetId;
    public CreatureCardPayload attacker;
    public CreatureCardPayload defender;
    private bool hasDefender;

    public CreatureCombatNetworkContainer(ulong initiatorId, ulong targetId, CreatureCardPayload attacker, CreatureCardPayload defender) {
        this.initiatorId = initiatorId;
        this.targetId = targetId;
        this.attacker = attacker;
        this.defender = defender;
        hasDefender = defender != null;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref initiatorId);
        serializer.SerializeValue(ref targetId);
        serializer.SerializeNetworkSerializable(ref attacker);
        serializer.SerializeValue(ref hasDefender);
        if(hasDefender)
            serializer.SerializeNetworkSerializable(ref defender);
    }
}