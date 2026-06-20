using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FieldUIManager : NetworkBehaviour {
    [SerializeField] private PlayingFieldUIManager playingFieldUIManager;
    [SerializeField] private CombatFieldUIManager combatFieldUIManager;
    private CombatManager combatManager;

    private void Start() {
        combatManager = ServiceLocator.Get<CombatManager>();

        EventBus.Instance.OnDeclareAttackerFinished += DeclareAttacker;
        EventBus.Instance.OnDeclareDefenderFinished += DeclareDefender;
        EventBus.Instance.OnUndeclareAttackerFinished += UndeclareAttacker;
        EventBus.Instance.OnUndeclareDefenderFinished += UndeclareDefender;
        combatManager.OnDuelistCombatFinsihed += ReleaseCombatCreatures;
    }

    private void DeclareAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI =  playingFieldUIManager.ReleaseCreature(args.InitiatorId, args.Card.Uuid);
        combatFieldUIManager.AddAttacker(args.TargetId, cardUI);
    }

    private void DeclareDefender(object sender, CreatureCombatPayloadEventArgs args) {
        CreatureFieldCardUI cardUI = playingFieldUIManager.ReleaseCreature(args.TargetId, args.Defender.Uuid);
        combatFieldUIManager.AddDefender(args.TargetId, args.Attacker.Uuid, cardUI);
    }

    private void UndeclareAttacker(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI = combatFieldUIManager.ReleaseAttacker(args.TargetId, args.Card.Uuid);
        playingFieldUIManager.AddCreatureCard(args.InitiatorId, cardUI);
    }

    private void UndeclareDefender(object sender, CombatCreaturePayloadEventArgs args) {
        CreatureFieldCardUI cardUI = combatFieldUIManager.ReleaseDefender(args.TargetId, args.Card.Uuid);
        playingFieldUIManager.AddCreatureCard(args.TargetId, cardUI);
    }

    private void ReleaseCombatCreatures(object sender, DuelistCombatEventArgs args) {
        List<CreatureFieldCardUI> attackers = combatFieldUIManager.ReleaseAttackers(args.TargetId);
        List<CreatureFieldCardUI> defenders = combatFieldUIManager.ReleaseDefenders(args.TargetId);
        playingFieldUIManager.AddCreatureCards(args.InitiatorId, attackers);
        playingFieldUIManager.AddCreatureCards(args.TargetId, defenders);
    }
}
