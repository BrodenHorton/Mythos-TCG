using System;
using UnityEngine;

public class OpponentCombatFieldUIController : CombatFieldUIController {
    [SerializeField] private CombatFieldUI combatFieldUI;

    public override void Init(MatchPlayer player) {
        target = player;
        combatFieldUI.Init(player.PlayerId);
    }

    public override void AddAttacker(CreatureCard attacker) {
        combatFieldUI.AddAttacker(attacker);
    }

    public override void AddDefender(CreatureCard defender, Guid attackerCardUuid) {
        combatFieldUI.AddDefender(defender, attackerCardUuid);
    }

    public override void RemoveAttacker(CreatureCard attacker) {
        combatFieldUI.RemoveAttacker(attacker.Uuid);
    }

    public override void RemoveDefender(CreatureCard defender) {
        combatFieldUI.RemoveDefender(defender.Uuid);
    }

    public override void ReleaseCreatureCards(DuelistCombat combat) {
        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            combat.Initiator,
            combatFieldUI.Attackers));
        EventBus.InvokeOnReleaseCombatCreatures(this, new ReleaseCombatCreaturesEventArgs(
            combat.Target,
            combatFieldUI.Defenders));
        combatFieldUI.ClearCreatures();
    }

    public override bool ContainsAttacker(Guid uuid) {
        return combatFieldUI.ContainsAttacker(uuid);
    }

    public override bool ContainsDefender(Guid uuid) {
        return combatFieldUI.ContainsDefender(uuid);
    }
}
