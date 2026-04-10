using System;
using Unity.Netcode;

public abstract class CombatFieldUIController : NetworkBehaviour {
    protected MatchPlayer target;
    protected DuelManager duelManager;
    protected DuelStateManager stateManager;

    protected virtual void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public abstract void Init(MatchPlayer player);

    public abstract void AddAttacker(CreatureCard attacker);

    public abstract void AddDefender(CreatureCard defender, Guid attackerCardUuid);

    public abstract void RemoveAttacker(CreatureCard attacker);

    public abstract void RemoveDefender(CreatureCard defender);

    public abstract void ReleaseCreatureCards(DuelistCombat combat);

    public abstract bool ContainsAttacker(Guid uuid);

    public abstract bool ContainsDefender(Guid uuid);

    public MatchPlayer Target { get { return target; } }
}