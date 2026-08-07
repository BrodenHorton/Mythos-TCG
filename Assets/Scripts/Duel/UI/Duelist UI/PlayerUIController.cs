using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerUIController : DuelistUIController {
    [SerializeField] private PlayerUI playerUI;
    
    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private ActionManager actionManager;
    private SpellChainManager spellChainManager;

    private void Start() {
        duelManager = ServiceLocator.Get<DuelManager>();
        stateManager = ServiceLocator.Get<DuelStateManager>();
        actionManager = ServiceLocator.Get<ActionManager>();
        spellChainManager = ServiceLocator.Get<SpellChainManager>();

        EventBus.Instance.OnPlayHandCard += PlayHandCard;
        EventBus.Instance.OnReleaseHandCardDrag += ResetHandCardPosition;
    }

    public override void Init(ulong playerId, int lifePoints, int manaCount) {
        this.playerId = playerId;
        playerUI.Init(playerId, lifePoints, manaCount);
    }

    public override void SetLifePoints(int lifePoints) {
        playerUI.SetLifePoints(lifePoints);
    }

    public override void SetManaCount(int manaCount) {
        playerUI.SetManaCount(manaCount);
    }

    public override void DrawCard(CardPayload card) {
        playerUI.DrawCard(card);
    }

    public override void RemoveCardFromHand(Guid cardUuid) {
        playerUI.RemoveCardFromHand(cardUuid);
    }

    private void PlayHandCard(object sender, PlayerCardUuidEventArgs args) {
        PlayHandCardServerRpc(args.PlayerId, args.CardUuid.ToString());
    }

    [Rpc(SendTo.Server)]
    private void PlayHandCardServerRpc(ulong playerId, FixedString128Bytes handCardUuidStr) {
        MatchPlayer player = duelManager.GetPlayerById(playerId);
        Guid handCardUuid = Guid.Parse(handCardUuidStr.ToString());
        if (!actionManager.ActionFocusPlayerIds.Contains(playerId))
            return;
        if (!player.ContainsHandCardeUuid(handCardUuid))
            return;
        if (!player.GetHandCardByUuid(handCardUuid).IsPlayable(duelManager, stateManager, spellChainManager, player))
            return;

        duelManager.PlayCardFromHand(playerId, handCardUuid);
    }

    private void ResetHandCardPosition(object sender, CardUIEventArgs<HandCardUI> args) {
        if (!playerUI.ContainsCard(args.CardUI.CardUuid))
            return;

        playerUI.SetDefaultCardPositions();
    }

    public override DuelistUI GetDuelistUI() {
        return playerUI;
    }
}
