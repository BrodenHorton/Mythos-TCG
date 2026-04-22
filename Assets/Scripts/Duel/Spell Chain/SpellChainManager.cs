using System;
using System.Collections.Generic;
using Unity.Netcode;

public class SpellChainManager : NetworkBehaviour {
    public event EventHandler<PlayerEventArgs> OnActionChainStart;
    public event EventHandler<PlayerEventArgs> OnPlayerSpellChainTurn;
    public event EventHandler<SpellCardAction> OnActionAddedToActionChain;
    public event EventHandler OnActionChainFinished;

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
        duelManager = GetComponent<DuelManager>();
        if (duelManager == null)
            throw new Exception("DuelManager not found on GameObject");
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
            StartSpellChainServerRpc(duelManager.GetPlayerIndex(args.Player), args.Card.GetNetworkSerializableObject());
        else
            AddSpellToChainServerRpc(args.Card.GetNetworkSerializableObject());
    }

    [Rpc(SendTo.Server)]
    private void StartSpellChainServerRpc(int playerIndex, SpellCardNetworkSerializable spellCardNetworkSerializable) {
        StartSpellChainClientRpc(playerIndex, spellCardNetworkSerializable);
        PassActionServerRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartSpellChainClientRpc(int playerIndex, SpellCardNetworkSerializable spellCardNetworkSerializable) {
        startingIndex = playerIndex;
        currentIndex = playerIndex;
        OnActionChainStart?.Invoke(this, new PlayerEventArgs(duelManager.Players[currentIndex]));
        SpellCard card = new SpellCard(spellCardNetworkSerializable);
        SpellCardAction action = new SpellCardAction(card, duelManager.Players[playerIndex]);
        spellChain.Push(action);
        OnActionAddedToActionChain?.Invoke(this, action);
    }

    [Rpc(SendTo.Server)]
    private void AddSpellToChainServerRpc(SpellCardNetworkSerializable spellCardNetworkSerializable) {
        AddSpellToChainClientRpc(spellCardNetworkSerializable);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddSpellToChainClientRpc(SpellCardNetworkSerializable spellCardNetworkSerializable) {
        SpellCard card = new SpellCard(spellCardNetworkSerializable);
        SpellCardAction action = new SpellCardAction(card, duelManager.Players[currentIndex]);
        spellChain.Push(action);
        OnActionAddedToActionChain?.Invoke(this, action);
        startingIndex = currentIndex;
        currentIndex++;
    }

    [Rpc(SendTo.Server)]
    private void PassActionServerRpc() {
        PassActionClientRpc();
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PassActionClientRpc() {
        currentIndex++;
        actionManager.ActionFocusPlayerIndex = currentIndex;

        if (currentIndex == startingIndex)
            ExecuteActionChain();
        else {
            if (duelManager.GetPlayerIndex(duelManager.LocalClientPlayer) == currentIndex)
                actionManager.AddAction(PassActionServerRpc, "Pass", "Waiting for Opponent");
            OnPlayerSpellChainTurn?.Invoke(this, new PlayerEventArgs(duelManager.Players[currentIndex]));
        }
    }

    private void ExecuteActionChain() {
        while(spellChain.Count > 0) {
            SpellCardAction action = spellChain.Pop();
            duelManager.ExecuteSpellServerRpc(duelManager.GetPlayerIndex(action.Initiator), action.Card.GetNetworkSerializableObject());
        }
        OnActionChainFinished?.Invoke(this, EventArgs.Empty);
    }
}
