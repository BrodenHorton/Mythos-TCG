using System;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPlayingFieldUIController : PlayingFieldUIController {
    [SerializeField] private PlayingFieldUI playingFieldUI;

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

    public override void RemoveCreature(Guid cardUuid) {
        playingFieldUI.RemoveCreature(card.Uuid);
    }

    public override void UpdateCreatureFieldCard(CreatureCardPayload card) {
        if (!ContainsCreature(card.Uuid))
            return;

        playingFieldUI.UpdateCreatureFieldCard(card);
    }

    public override void TapCreature(Guid cardUuid) {
        playingFieldUI.TapCreature(card);
    }

    public override void UntapCreature(Guid cardUuid) {
        playingFieldUI.UntapCreature(card);
    }

    public override void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(creatures[i]);
    }

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
