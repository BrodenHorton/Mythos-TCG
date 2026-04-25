using System;
using System.Collections.Generic;
using Unity.Netcode;

public class SpellChainManager : NetworkBehaviour {
    public event EventHandler<PlayerEventArgs> OnSpellChainStart;
    public event EventHandler<PlayerEventArgs> OnPlayerSpellChainTurn;
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

        EventBus.OnActionChainSpellCardPlayed += AddSpellToChain;
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
        if (currentIndex == duelManager.GetLocalClientPlayerIndex())
            actionManager.PopAction();

        if(IsServer)
            PassActionServerRpc();
    }

    [Rpc(SendTo.Server)]
    private void PassActionServerRpc() {
        PassActionClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PassActionClientRpc() {
        currentIndex = (currentIndex + 1) % duelManager.Players.Count;

        if (currentIndex == startingIndex) {
            ExecuteActionChain();
            actionManager.SetActionFocusPlayerIndices(duelManager.GetPlayerIndex(duelManager.GetCurrentPlayerTurn()));
        }
        else {
            if (duelManager.GetPlayerIndex(duelManager.LocalClientPlayer) == currentIndex)
                actionManager.AddAction(PassActionServerRpc, "Pass", "Waiting for Opponent");
            actionManager.SetActionFocusPlayerIndices(currentIndex);
            OnPlayerSpellChainTurn?.Invoke(this, new PlayerEventArgs(duelManager.Players[currentIndex]));
        }
    }

    private void ExecuteActionChain() {
        while(spellChain.Count > 0) {
            SpellCardAction action = spellChain.Pop();
            duelManager.ExecuteSpell(action.Initiator, action.Card);
            OnSpellRemovedFromSpellChain?.Invoke(this, action);
        }
        OnSpellChainFinished?.Invoke(this, EventArgs.Empty);
    }

    public bool IsSpellChainActive() {
        return spellChain.Count > 0;
    }
}
