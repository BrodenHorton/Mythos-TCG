using Unity.Netcode;

public struct CreatureCombatPayload : INetworkSerializable {
    public CreatureCardPayload attackerPayload;
    public CreatureCardPayload defenderPayload;
    public bool hasDefender; // Included so we know if we should serialize and deserialize the defender

    public CreatureCombatPayload(CreatureCombat creatureCombat) {
        attackerPayload = new CreatureCardPayload(creatureCombat.Attacker);
        defenderPayload = creatureCombat.Defender != null ? new CreatureCardPayload(creatureCombat.Defender) : null;
        hasDefender = defenderPayload != null;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeNetworkSerializable(ref attackerPayload);
        serializer.SerializeValue(ref hasDefender);
        if (hasDefender)
            serializer.SerializeNetworkSerializable(ref defenderPayload);
    }
}