using System;
using System.Collections.Generic;
using Unity.Netcode;

public class SpellChainManager : NetworkBehaviour {
    public event EventHandler<PlayerEventArgs> OnSpellChainStart;
    public event EventHandler<PlayerEventArgs> OnSpellChainTurnEnd;
    public event EventHandler<PlayerCardPayloadEventArgs<SpellCardPayload>> OnSpellAddedToSpellChain;
    public event EventHandler<PlayerCardPayloadEventArgs<SpellCardPayload>> OnSpellRemovedFromSpellChain;
    public event EventHandler OnSpellChainFinished;

    private DuelManager duelManager;
    private ActionManager actionManager;
    private int startingIndex;
    private int currentIndex;
    private Stack<SpellCardAction> spellChain;

    private void Awake() {
        startingIndex = 0;
        currentIndex = 0;
        spellChain = new Stack<SpellCardAction>();

        ServiceLocator.Register(this);
    }

    private void Start() {
        if (!IsServer)
            return;

        duelManager = ServiceLocator.Get<DuelManager>();
        actionManager = ServiceLocator.Get<ActionManager>();

        EventBus.Instance.OnSpellChainCardPlayed += AddSpellToChain;
    }

    private void AddSpellToChain(object sender, PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            return;
        if (args.Card.SpellType == SpellType.Instant)
            throw new Exception("Instant spells should not be added to an action chain");
        if (args.Card.SpellType == SpellType.Slow && spellChain.Count != 0)
            throw new Exception("Slow spells can only start an action chain");

        MatchPlayer player = duelManager.GetPlayerById(args.PlayerId);
        if (spellChain.Count == 0)
            StartSpellChain(player, args.Card);
        else
            AddSpellToChain(player, args.Card);
    }

    private void StartSpellChain(MatchPlayer player, SpellCard spellCard) {
        if (!IsServer)
            return;

        int playerIndex = duelManager.GetPlayerIndex(player);
        startingIndex = playerIndex;
        currentIndex = playerIndex;
        InvokeOnSpellChainStartClientRpc(player.PlayerId);
        SpellCardAction action = new SpellCardAction(spellCard, player.PlayerId);
        spellChain.Push(action);
        InvokeOnSpellAddedToSpellChainClientRpc(player.PlayerId, new SpellCardPayload(action.Card));
        PassAction();
    }

    private void AddSpellToChain(MatchPlayer player, SpellCard spellCard) {
        if (!IsServer)
            return;

        SpellCardAction action = new SpellCardAction(spellCard, player.PlayerId);
        spellChain.Push(action);
        InvokeOnSpellAddedToSpellChainClientRpc(player.PlayerId, new SpellCardPayload(action.Card));
        startingIndex = currentIndex;
        PassAction();
    }

    public void PassAction() {
        if (!IsServer)
            return;

        InvokeOnSpellChainTurnEndClientRpc(duelManager.Players[currentIndex].PlayerId);
        IncrementCurrentIndex();
        if (currentIndex == startingIndex) {
            ExecuteActionChain();
            actionManager.SetActionFocusPlayerIndices(duelManager.GetCurrentPlayerTurn().PlayerId);
            InvokeOnSpellChainFinishedClientRpc();
        }
        else {
            ulong currentIndexPlayerId = duelManager.Players[currentIndex].PlayerId;
            PassSpellChainDuelistAction duelistAction = new PassSpellChainDuelistAction(currentIndexPlayerId, this);
            actionManager.AddAction(currentIndexPlayerId, duelistAction);
            actionManager.SetActionFocusPlayerIndices(currentIndexPlayerId);
        }
    }

    private void IncrementCurrentIndex() {
        if (!IsServer)
            return;

        currentIndex = (currentIndex + 1) % duelManager.Players.Count;
    }

    private void ExecuteActionChain() {
        if (!IsServer)
            return;

        while(spellChain.Count > 0) {
            SpellCardAction action = spellChain.Pop();
            duelManager.ExecuteSpell(duelManager.GetPlayerById(action.InitiatorId), action.Card);
            InvokeOnSpellRemovedFromSpellChainClientRpc(action.InitiatorId, new SpellCardPayload(action.Card));
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellChainStartClientRpc(ulong playerId) {
        OnSpellChainStart?.Invoke(this, new PlayerEventArgs(playerId));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellAddedToSpellChainClientRpc(ulong playerId, SpellCardPayload spellCard) {
        PlayerCardPayloadEventArgs<SpellCardPayload> args = new PlayerCardPayloadEventArgs<SpellCardPayload>(playerId, spellCard);
        OnSpellAddedToSpellChain?.Invoke(this, args);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellRemovedFromSpellChainClientRpc(ulong playerId, SpellCardPayload spellCard) {
        PlayerCardPayloadEventArgs<SpellCardPayload> args = new PlayerCardPayloadEventArgs<SpellCardPayload>(playerId, spellCard);
        OnSpellRemovedFromSpellChain?.Invoke(this, args);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellChainTurnEndClientRpc(ulong playerId) {
        OnSpellChainTurnEnd?.Invoke(this, new PlayerEventArgs(playerId));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellChainFinishedClientRpc() {
        OnSpellChainFinished?.Invoke(this, EventArgs.Empty);
    }

    public bool IsSpellChainActive() {
        return spellChain.Count > 0;
    }
}
