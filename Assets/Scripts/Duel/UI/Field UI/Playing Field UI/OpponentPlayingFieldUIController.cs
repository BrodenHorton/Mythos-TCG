using System;
using System.Collections.Generic;
using UnityEngine;

public class OpponentPlayingFieldUIController : PlayingFieldUIController {
    [SerializeField] private PlayingFieldUI playingFieldUI;

    public override void Init(MatchPlayer player) {
        this.player = player;
        playingFieldUI.Init(player.PlayerId);
    }

    public override void PlayCreatureCard(CreatureCard card) {
        playingFieldUI.PlayCreatureCard(player, card);
    }

    public override void PlayDomainCard(DomainCard card) {
        playingFieldUI.PlayDomainCard(card);
    }

    public override void RemoveCreature(CreatureCard card) {
        playingFieldUI.RemoveCreature(card.Uuid);
    }

    public override void UpdateCreatureFieldCard(CreatureCard card) {
        if (!ContainsCreature(card.Uuid))
            return;

        playingFieldUI.UpdateCreatureFieldCard(card);
    }

    public override void TapCreature(CreatureCard card) {
        playingFieldUI.TapCreature(card);
    }

    public override void UntapCreature(CreatureCard card) {
        playingFieldUI.UntapCreature(card);
    }

    public override void GetCreatureCardsFromCombat(List<CreatureFieldCardUI> creatures) {
        for (int i = 0; i < creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(player, creatures[i]);
    }

    public override bool ContainsCreature(Guid uuid) {
        return playingFieldUI.ContainsCreature(uuid);
    }
}
