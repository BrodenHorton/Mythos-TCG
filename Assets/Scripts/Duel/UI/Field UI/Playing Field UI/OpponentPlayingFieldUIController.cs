using System;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPlayingFieldUIController : PlayingFieldUIController {

    public override void Init(ulong playerId) {
        this.playerId = playerId;
        playingFieldUI.Init(playerId);
    }

    public override void PlayCreatureCard(CreatureCardPayload card) {
        playingFieldUI.PlayCreatureCard(card);
    }

    public override void PlayDomainCard(DomainCardPayload card) {
        playingFieldUI.PlayDomainCard(card);
    }

    public override void AddCreatureCard(CreatureFieldCardUI card) {
        playingFieldUI.AddCreatureFieldCard(card);
    }

    public override void RemoveCreature(Guid cardUuid) {
        playingFieldUI.RemoveCreature(cardUuid);
    }

    public override CreatureFieldCardUI ReleaseCreature(Guid cardUuid) {
        return playingFieldUI.ReleaseCreature(cardUuid);
    }

    public override void AddCreatureCards(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
