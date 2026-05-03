using System;
using System.Collections.Generic;
using Unity.Netcode;

public abstract class PlayingFieldUIController : NetworkBehaviour {
    protected ulong playerId;
    protected DuelManager duelManager;
    protected DuelStateManager stateManager;
    protected CombatStateManager combatStateManager;

    protected virtual void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");
        combatStateManager = FindFirstObjectByType<CombatStateManager>();
        if (combatStateManager == null)
            throw new Exception("Could not find DuelStateManager object");
    }

    public abstract void Init(ulong playerId);

    public abstract void PlayCreatureCard(CreatureCard card);

    public abstract void PlayDomainCard(DomainCard card);

    public abstract void RemoveCreature(CreatureCard card);

    public abstract void UpdateCreatureFieldCard(CreatureCard card);

    public abstract void TapCreature(CreatureCard card);

    public abstract void UntapCreature(CreatureCard card);

    public abstract void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures);

    public abstract bool ContainsCreature(Guid uuid);
}