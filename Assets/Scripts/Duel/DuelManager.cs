using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DuelManager : NetworkBehaviour {
    private static readonly int STARTING_LIFE_POINTS = 20;
    private static readonly int STARTING_MANA_COUNT = 0;
    public static readonly int INITIAL_HAND_SIZE = 5;

    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitialization;
    public event EventHandler OnPlayersInitializationFinished;
    public event EventHandler<NextPlayerTurnEventArgs> OnNextPlayerTurn;
    public event EventHandler<int> OnNextPlayerTurnClient;
    public event EventHandler<int> OnNextFullTurn;

    [SerializeField] private DeckSimulation deckSim;

    private List<MatchPlayer> players;
    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        currentPlayerTurnIndex = 0;
        fullTurnCount = 1;

        ServiceLocator.Register(this);
    }

    private void Start() {
        if (!IsServer)
            return;

        GameManager.Instance.OnGameStart += InitializePlayers;
        EventBus.Instance.OnCreatureCardSelectedForPlay += PlayCreatureCardFromHand;
        EventBus.Instance.OnDomainCardSelectedForPlay += PlayDomainCardFromHand;
        EventBus.Instance.OnSpellCardSelectedForPlay += PlaySpellCardFromHand;
        EventBus.Instance.OnSpellCardPlayedFromHand += PlaySpellCard;
    }

    private void InitializePlayers(object sender, StartGameEventArgs args) {
        if (!IsServer)
            return;

        InitializePlayers(args.PlayerOrder.ToArray());
    }

    private void InitializePlayers(ulong[] playerOrder) {
        if (!IsServer)
            return;

        players = new List<MatchPlayer>();
        for (int i = 0; i < playerOrder.Length; i++) {
            List<Card> deck = deckSim != null ? deckSim.GenerateDeck() : Temp_PopulateDeck();
            MatchPlayer player = new MatchPlayer(playerOrder[i], deck);
            players.Add(player);
        }

        for(int i = 0; i < playerOrder.Length; i++) {
            BaseRpcTarget target = RpcTarget.Group(new List<ulong>() { playerOrder[i] }, RpcTargetUse.Temp);
            InvokePlayerInitializationClientRpc(playerOrder, i, target);
        }
        OnPlayersInitializationFinished?.Invoke(this, EventArgs.Empty);
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void InvokePlayerInitializationClientRpc(ulong[] playerOrder, int localClientPlayerIndex, RpcParams rpcParams) {
        OnPlayersInitialization?.Invoke(this, new PlayersInitializedEventArgs(new List<ulong>(playerOrder), localClientPlayerIndex, STARTING_LIFE_POINTS, STARTING_MANA_COUNT));
    }

    private List<Card> Temp_PopulateDeck() {
        List<Card> result = new List<Card>();
        int tempDeckSize = 40;
        int databaseCardCount = CardDatabase.Instance.Cards.Count;
        for (int i = 0; i < tempDeckSize; i++) {
            Card card = CardDatabase.Instance.Cards[UnityEngine.Random.Range(0, databaseCardCount)].GenerateCardFromBase();
            result.Add(card);
        }

        return result;
    }

    public void PlayCardFromHand(ulong playerId, Guid handCardUuid) {
        if (!IsServer)
            return;
        MatchPlayer player = GetPlayerById(playerId);
        if (!player.ContainsHandCardeUuid(handCardUuid))
            throw new Exception("Attmepting to play card with uuid that is not in the player's hand: " + handCardUuid);

        player.GetHandCardByUuid(handCardUuid).PlayCardFromHand(player);
    }

    public void PlayCreatureCardFromHand(object sender, PlayerCardEventArgs<CreatureCard> args) {
        if (!IsServer)
            return;

        MatchPlayer player = GetPlayerById(args.PlayerId);
        player.PlayCreatureCardFromHand(args.Card);
    }
    
    public void PlayDomainCardFromHand(object sender, PlayerCardEventArgs<DomainCard> args) {
        if (!IsServer)
            return;

        MatchPlayer player = Players[GetPlayerIndex(args.PlayerId)];
        player.PlayDomainCardFromHand(args.Card);
    }

    public void PlaySpellCardFromHand(object sender, PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            return;

        MatchPlayer player = Players[GetPlayerIndex(args.PlayerId)];
        player.PlaySpellCardFromHand(args.Card);
    }

    public void PlaySpellCard(object sender, PlayerCardEventArgs<SpellCard> args) {
        if (!IsServer)
            return;

        MatchPlayer player = Players[GetPlayerIndex(args.PlayerId)];
        if (args.Card.SpellType == SpellType.Instant)
            ExecuteSpell(player, args.Card);
        else
            EventBus.Instance.InvokeOnSpellChainCardPlayed(new PlayerCardEventArgs<SpellCard>(args.PlayerId, args.Card));
    }

    public void ExecuteSpell(MatchPlayer player, SpellCard spellCard) {
        if (!IsServer)
            return;

        for (int i = 0; i < spellCard.BaseEffects.Count; i++) {
            spellCard.BaseEffects[i].Execute();
            // TODO: Execute the additional effects on the SpellCard class
        }
    }

    public void NextTurn() {
        if (!IsServer)
            return;

        RegenerateCreaturesHealth();
        currentPlayerTurnIndex = ++currentPlayerTurnIndex % players.Count;
        InvokeOnNextPlayerTurnClientClientRpc(currentPlayerTurnIndex);
        OnNextPlayerTurn?.Invoke(this, new NextPlayerTurnEventArgs(GetCurrentPlayerTurn(), currentPlayerTurnIndex));
        if (currentPlayerTurnIndex == 0) {
            fullTurnCount++;
            InvokeOnNextFullTurnClientRpc(fullTurnCount);
        }
    }

    private void RegenerateCreaturesHealth() {
        if (!IsServer)
            return;

        foreach (MatchPlayer player in Players) {
            for (int i = 0; i < player.Creatures.Count; i++) {
                if (player.Creatures[i].CurrentDamage > 0)
                    player.Creatures[i].CurrentDamage = 0;
            }
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnNextPlayerTurnClientClientRpc(int playerIndex) {
        OnNextPlayerTurnClient?.Invoke(this, playerIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InvokeOnNextFullTurnClientRpc(int fullTurnCount) {
        OnNextFullTurn?.Invoke(this, fullTurnCount);
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer GetCurrentPlayerTurn() {
        return players[currentPlayerTurnIndex];
    }

    public int GetStartOfTurnManaCount() {
        return fullTurnCount;
    }

    public void IncrementFullTurnCount() {
        fullTurnCount++;
    }

    public MatchPlayer GetPlayerById(ulong playerId) {
        foreach (MatchPlayer p in players) {
            if (p.PlayerId == playerId)
                return p;
        }

        throw new Exception("Unable to find player with playerId: " + playerId);
    }

    public int GetPlayerIndex(MatchPlayer player) {
        return GetPlayerIndex(player.PlayerId);
    }

    public int GetPlayerIndex(ulong playerId) {
        for(int i = 0; i < players.Count; i++) {
            if (players[i].PlayerId == playerId)
                return i;
        }

        throw new Exception("Player index could not be found for playerId: " + playerId);
    }

    public List<ulong> GetPlayerIds() {
        List<ulong> playerIds = new List<ulong>();
        foreach(MatchPlayer player in players) {
            playerIds.Add(player.PlayerId);
        }

        return playerIds;
    }

    public List<MatchPlayer> Players { get { return players; } }

    public int CurrentPlayerTurnIndex {  get { return currentPlayerTurnIndex; } }

    public int FullTurnCount { get { return fullTurnCount; } }
}
