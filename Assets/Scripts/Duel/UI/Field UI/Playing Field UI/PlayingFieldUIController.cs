using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayingFieldUIController : NetworkBehaviour {
    [SerializeField] private PlayingFieldUI playingFieldUI;
    private ulong playerId;

    public void Init(ulong playerId) {
        this.playerId = playerId;
        playingFieldUI.Init(playerId);
    }

    public void PlayCreatureCard(CreatureCardPayload card) {
        playingFieldUI.PlayCreatureCard(card);
    }

    public void PlayDomainCard(DomainCardPayload card) {
        playingFieldUI.PlayDomainCard(card);
    }

    public void AddCreatureCard(CreatureFieldCardUI card) {
        playingFieldUI.AddCreatureFieldCard(card);
    }

    public void AddCreatureCards(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public void RemoveCreature(Guid cardUuid) {
        playingFieldUI.RemoveCreature(cardUuid);
    }

    public CreatureFieldCardUI ReleaseCreature(Guid cardUuid) {
        return playingFieldUI.ReleaseCreature(cardUuid);
    }

    public bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}