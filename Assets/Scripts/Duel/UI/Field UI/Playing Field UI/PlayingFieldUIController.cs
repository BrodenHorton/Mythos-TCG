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

    private void SelectCardDrag(object sender, PlayingFieldCardDragEventArgs args) {
        if(!CanSelectAttacker(args) && !CanSelectDefender(args))
            args.IsCancelled = true;
    }

    private bool CanSelectAttacker(PlayingFieldCardDragEventArgs args) {
        if (player != duelManager.LocalClientPlayer)
            return false;
        if (duelManager.IsLocalClientPlayerTurn())
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

    private bool CanSelectDefender(PlayingFieldCardDragEventArgs args) {
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

    private void DeclareAttacker(object sender, ReleaseCreatureFieldCardDragOverCombatAreaEventArgs args) {
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
        EventBus.InvokeOnDelcareAttacker(this, new DeclareAttackerEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], creatureCard));
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