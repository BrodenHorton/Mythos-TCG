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

        playingFieldUI.OnSelectCardDrag += SelectCardDrag;
        playingFieldUI.OnReleaseCardDrag += ReleaseCardDrag;
    }

    public void Init(MatchPlayer player) {
        this.player = player;
        playingFieldUI.Init(player.PlayerId);
    }

    public void PlayCreatureCard(CreatureCard card) {
        playingFieldUI.PlayCreatureCard(card);
    }

    public void PlayDomainCard(SpellCard card) {
        playingFieldUI.PlayDomainCard(card);
    }

    public void RemoveAttacker(CreatureCard attacker) {
        if (!playingFieldUI.ContainsCreature(attacker.Uuid))
            throw new Exception("Playing field UI controller does not contain card uuid: " + attacker.Uuid);

        playingFieldUI.RemoveCreature(attacker.Uuid);
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

    private void SelectCardDrag(object sender, SelectFieldCardDragEventArgs args) {
        if (!duelManager.IsLocalClientPlayerTurn()) {
            args.IsCancelled = true;
            return;
        }
        if (player != duelManager.LocalClientPlayer) {
            args.IsCancelled = true;
            return;
        }
        if (stateManager.CurrentState != stateManager.CombatPhase) {
            args.IsCancelled = true;
            return;
        }
        if (args.CardUI == null) {
            args.IsCancelled = true;
            return;
        }
        if (!player.ContainsCreatureUuid(args.CardUI.CardUuid)) {
            args.IsCancelled = true;
            return;
        }
        CreatureCard creatureCard = player.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null) {
            args.IsCancelled = true;
            return;
        }
        if (!creatureCard.CanAttack()) {
            args.IsCancelled = true;
            return;
        }
    }

    private void ReleaseCardDrag(object sender, ReleaseFieldCardDragEventArgs args) {
        if (!args.IsReleasedInCombatArea)
            return;
        if (player.PlayerId != duelManager.GetCurrentPlayerTurn().PlayerId)
            return;
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        CreatureCard creatureCard = player.GetCreatureByUuid(args.CardUI.CardUuid);
        if (creatureCard == null)
            return;
        if (!creatureCard.CanAttack())
            return;

        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        // TODO: Change target to the passed in target instead of hard coding it
        MatchPlayer target = duelManager.Players[(duelManager.GetPlayerIndex(initiator.PlayerId) + 1) % duelManager.Players.Count];
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
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }
}