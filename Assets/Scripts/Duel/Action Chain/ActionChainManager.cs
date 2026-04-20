using System;
using System.Collections.Generic;
using Unity.Netcode;

public class ActionChainManager : NetworkBehaviour {
    public event EventHandler<ulong> OnActionChainStart;
    public event EventHandler<SpellCardAction> OnActionAddedToActionChain;
    public event EventHandler OnActionChainFinished;

    private DuelManager duelManager;
    private int startingIndex;
    private int currentIndex;
    private Stack<SpellCardAction> actionChain;

    private void Awake() {
        startingIndex = 0;
        currentIndex = 0;
        actionChain = new Stack<SpellCardAction>();
    }

    private void Start() {
        duelManager = GetComponent<DuelManager>();
        if (duelManager == null)
            throw new Exception("DuelManager not found on GameObject");

        EventBus.OnActionChainSpellCardPlayed += AddAction;
    }

    private void AddAction(object sender, PlayerCardEventArgs<SpellCard> args) {
        if (args.Card.SpellType == SpellType.Instant)
            throw new Exception("Instant spells should not be added to an action chain");
        if (args.Card.SpellType == SpellType.Slow && actionChain.Count != 0)
            throw new Exception("Slow spells can only start an action chain");

        if (actionChain.Count == 0)
            StartActionChainServerRpc(duelManager.GetPlayerIndex(args.Player), args.Card.GetNetworkSerializableObject());
        else
            AddActionServerRpc(args.Card.GetNetworkSerializableObject());
    }

    [Rpc(SendTo.Server)]
    private void StartActionChainServerRpc(int playerIndex, SpellCardNetworkSerializable spellCardNetworkSerializable) {
        StartActionChainClientRpc(playerIndex, spellCardNetworkSerializable);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void StartActionChainClientRpc(int playerIndex, SpellCardNetworkSerializable spellCardNetworkSerializable) {
        startingIndex = playerIndex;
        currentIndex = playerIndex;
        OnActionChainStart?.Invoke(this, duelManager.Players[currentIndex].PlayerId);
        SpellCard card = new SpellCard(spellCardNetworkSerializable);
        SpellCardAction action = new SpellCardAction(card, duelManager.Players[playerIndex]);
        actionChain.Push(action);
        OnActionAddedToActionChain?.Invoke(this, action);
        currentIndex++;
    }

    [Rpc(SendTo.Server)]
    private void AddActionServerRpc(SpellCardNetworkSerializable spellCardNetworkSerializable) {
        AddActionClientRpc(spellCardNetworkSerializable);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void AddActionClientRpc(SpellCardNetworkSerializable spellCardNetworkSerializable) {
        SpellCard card = new SpellCard(spellCardNetworkSerializable);
        SpellCardAction action = new SpellCardAction(card, duelManager.Players[currentIndex]);
        actionChain.Push(action);
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

        if (currentIndex == startingIndex)
            ExecuteActionChain();
    }

    private void ExecuteActionChain() {
        while(actionChain.Count > 0) {
            SpellCardAction action = actionChain.Pop();
            duelManager.ExecuteSpellServerRpc(duelManager.GetPlayerIndex(action.Initiator), action.Card.GetNetworkSerializableObject());
        }
        OnActionChainFinished?.Invoke(this, EventArgs.Empty);
    }
}
