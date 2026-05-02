using System;
using System.Collections.Generic;
using Unity.Netcode;

public class SpellChainManager : NetworkBehaviour {
    public event EventHandler<PlayerEventArgs> OnSpellChainStart;
    public event EventHandler<PlayerEventArgs> OnSpellChainTurnEnd;
    public event EventHandler<SpellCardAction> OnSpellAddedToSpellChain;
    public event EventHandler<SpellCardAction> OnSpellRemovedFromSpellChain;
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
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        actionManager = FindFirstObjectByType<ActionManager>();
        if (actionManager == null)
            throw new Exception("Could not find ActionManager object");

        EventBus.OnSpellChainCardPlayed += AddSpellToChain;
    }

    private void AddSpellToChain(object sender, PlayerCardEventArgs<SpellCard> args) {
        if (args.Card.SpellType == SpellType.Instant)
            throw new Exception("Instant spells should not be added to an action chain");
        if (args.Card.SpellType == SpellType.Slow && spellChain.Count != 0)
            throw new Exception("Slow spells can only start an action chain");

        if (spellChain.Count == 0)
            StartSpellChain(args.Player, args.Card);
        else
            AddSpellToChain(args.Player, args.Card);
    }

    private void StartSpellChain(MatchPlayer player, SpellCard spellCard) {
        int playerIndex = duelManager.GetPlayerIndex(player);
        startingIndex = playerIndex;
        currentIndex = playerIndex;
        OnSpellChainStart?.Invoke(this, new PlayerEventArgs(duelManager.Players[currentIndex]));
        SpellCardAction action = new SpellCardAction(spellCard, duelManager.Players[playerIndex]);
        spellChain.Push(action);
        OnSpellAddedToSpellChain?.Invoke(this, action);

        if(IsServer)
            PassActionServerRpc();
    }

    private void AddSpellToChain(MatchPlayer player, SpellCard spellCard) {
        SpellCardAction action = new SpellCardAction(spellCard, duelManager.Players[currentIndex]);
        spellChain.Push(action);
        OnSpellAddedToSpellChain?.Invoke(this, action);
        startingIndex = currentIndex;

        if(IsServer)
            PassActionServerRpc();
    }

    [Rpc(SendTo.Server)]
    public void PassActionServerRpc() {
        InvokeOnSpellChainTurnEndClientRpc();
        IncrementCurrentIndexClientRpc();
        if (currentIndex == startingIndex) {
            ExecuteActionChainClientRpc();
            actionManager.SetActionFocusPlayerIndicesServerRpc(duelManager.CurrentPlayerTurnIndex);
            InvokeOnSpellChainFinishedClientRpc();
        }
        else {
            List<ulong> currentIndexPlayerId = new List<ulong> {
                duelManager.Players[currentIndex].PlayerId
            };
            BaseRpcTarget rpcTarget = RpcTarget.Group(currentIndexPlayerId, RpcTargetUse.Temp);
            AddPassActionToPlayerClientRpc(rpcTarget);
            actionManager.SetActionFocusPlayerIndicesServerRpc(currentIndex);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void IncrementCurrentIndexClientRpc() {
        currentIndex = (currentIndex + 1) % duelManager.Players.Count;
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void AddPassActionToPlayerClientRpc(RpcParams rpcParams) {
        PassSpellChainDuelistAction duelistAction = new PassSpellChainDuelistAction(duelManager, this);
        actionManager.AddAction(duelistAction);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void ExecuteActionChainClientRpc() {
        while(spellChain.Count > 0) {
            SpellCardAction action = spellChain.Pop();
            duelManager.ExecuteSpell(action.Initiator, action.Card);
            OnSpellRemovedFromSpellChain?.Invoke(this, action);
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellChainTurnEndClientRpc() {
        OnSpellChainTurnEnd?.Invoke(this, new PlayerEventArgs(duelManager.Players[currentIndex]));
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnSpellChainFinishedClientRpc() {
        OnSpellChainFinished?.Invoke(this, EventArgs.Empty);
    }

    public bool IsSpellChainActive() {
        return spellChain.Count > 0;
    }
}
