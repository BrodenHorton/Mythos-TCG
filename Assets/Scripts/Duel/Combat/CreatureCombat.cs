using Unity.Netcode;

public class CreatureCombat : INetworkSerializable {
    private CreatureCard attacker;
    private CreatureCard defender;
    private bool hasDefender; // Included so we know if we should serialize and deserialize the defender

    public CreatureCombat() { }

    public CreatureCombat(CreatureCard attacker) {
        this.attacker = attacker;
        defender = null;
        hasDefender = false;
    }

    public CreatureCombat(CreatureCard attacker, CreatureCard defender) {
        this.attacker = attacker;
        this.defender = defender;
        hasDefender = defender != null;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeNetworkSerializable(ref attacker);
        serializer.SerializeValue(ref hasDefender);
        if(hasDefender)
            serializer.SerializeNetworkSerializable(ref defender);
    }

    public CreatureCard Attacker { get { return attacker; } }

    public CreatureCard Defender {
        get {
            return defender;
        }
        set {
            defender = value;
            hasDefender = defender != null;
        }
    }
}