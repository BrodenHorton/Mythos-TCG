using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayingFieldUIController : NetworkBehaviour {
    [SerializeField] private PlayingFieldUI playingFieldUI;

    private MatchPlayer player;
    private DuelManager duelManager;
    private DuelStateManager stateManager;

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        playingFieldUI.OnSelectingCardDrag += SelectCardDrag;
        // These dont go in the DuelistUIManager since they only apply to the local client player
        EventBus.OnReleaseCreatureFieldCardOverCombatArea += DeclareAttacker;
        EventBus.OnSelectAttackerToDefend += DeclareDefender;
    }

    public void Init(MatchPlayer player) {
        this.player = player;
        playingFieldUI.Init(player.PlayerId);
    }

    public void PlayCreatureCard(CreatureCard card) {
        playingFieldUI.PlayCreatureCard(player, card);
    }

    public void PlayDomainCard(SpellCard card) {
        playingFieldUI.PlayDomainCard(card);
    }

    public void RemoveCreature(CreatureCard card) {
        if (!playingFieldUI.ContainsCreature(card.Uuid))
            throw new Exception("Playing field UI controller does not contain card uuid: " + card.Uuid);

        playingFieldUI.RemoveCreature(card.Uuid);
    }

    public void TapCreature(CreatureCard card) {
        if (!playingFieldUI.ContainsCreature(card.Uuid))
            throw new Exception("Playing field UI controller does not contain card uuid: " + card.Uuid);

        playingFieldUI.TapCreature(card);
    }

    public void UntapCreature(CreatureCard card) {
        if (!playingFieldUI.ContainsCreature(card.Uuid))
            throw new Exception("Playing field UI controller does not contain card uuid: " + card.Uuid);

        playingFieldUI.UntapCreature(card);
    }

    private void SelectCardDrag(object sender, CreatureFieldCardDragEventArgs args) {
        if (!CanSelectAttacker(args) && !CanSelectDefender(args))
            args.IsCancelled = true;
    }

    private bool CanSelectAttacker(CreatureFieldCardDragEventArgs args) {
        if (player != duelManager.LocalClientPlayer)
            return false;
        if (!duelManager.IsLocalClientPlayerTurn())
            return false;
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return false;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareAttackers)
            return false;
        if (args.CardUI == null)
            return false;
        if (!player.ContainsCreatureUuid(args.CardUI.CardUuid))
            return false;
        CreatureCard creatureCard = player.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return false;
        if (!creatureCard.CanAttack())
            return false;

        return true;
    }

    private bool CanSelectDefender(CreatureFieldCardDragEventArgs args) {
        if (player != duelManager.LocalClientPlayer)
            return false;
        if (duelManager.IsLocalClientPlayerTurn())
            return false;
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return false;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareDefenders)
            return false;
        if (args.CardUI == null)
            return false;
        if (!player.ContainsCreatureUuid(args.CardUI.CardUuid))
            return false;
        CreatureCard creatureCard = player.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return false;

        return true;
    }

    private void DeclareAttacker(object sender, CreatureFieldCardEnteringCombatFieldEventArgs args) {
        if (player != duelManager.LocalClientPlayer)
            return;
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareAttackers)
            return;
        CreatureCard creatureCard = player.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return;
        if (!creatureCard.CanAttack())
            return;

        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        MatchPlayer target = duelManager.GetPlayerById(args.TargetPlayerId);
        DeclareAttackerServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), creatureCard.Uuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareAttackerServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        DeclareAttackerClientRpc(initiatorIndex, targetIndex, cardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeclareAttackerClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        CreatureCard creatureCard = duelManager.Players[initiatorIndex].GetCreatureByUuid(cardUuid);
        EventBus.InvokeOnDeclareAttacker(this, new DeclareAttackerEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], creatureCard));
    }

    private void DeclareDefender(object sender, SelectAttackerToDefendEventArgs args) {
        if (player != duelManager.LocalClientPlayer)
            return;
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        if (stateManager.CombatPhase.CombateState != CombatPhase.CombatState.DeclareDefenders)
            return;
        if (args.CombatFieldUI.TargetPlayerId != player.PlayerId)
            return;
        if (!player.ContainsCreatureUuid(args.Defender.CardUuid))
            return;
        CreatureCard defender = player.GetCreatureByUuid(args.Defender.CardUuid);
        if (defender == null)
            return;
        if (!defender.CanDefend())
            return;

        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        MatchPlayer target = duelManager.GetPlayerById(args.TargetPlayerId);
        DeclareDefenderServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), args.Attacker.CardUuid.ToString(), defender.Uuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void DeclareDefenderServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes attackerCardUuidStr, FixedString128Bytes defenderCardUuidStr) {
        DeclareDefenderClientRpc(initiatorIndex, targetIndex, attackerCardUuidStr, defenderCardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeclareDefenderClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes attackerCardUuidStr, FixedString128Bytes defenderCardUuidStr) {
        Guid attackerUuid = Guid.Parse(attackerCardUuidStr.ToString());
        Guid defenderUuid = Guid.Parse(defenderCardUuidStr.ToString());
        CreatureCard attacker = duelManager.Players[initiatorIndex].GetCreatureByUuid(attackerUuid);
        CreatureCard defender = duelManager.Players[targetIndex].GetCreatureByUuid(defenderUuid);
        EventBus.InvokeOnDeclareDefender(this, new DeclareDefenderEventArgs(duelManager.Players[targetIndex], defender, attacker));
    }

    public void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(player, creatures[i]);
    }

    public bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }
}