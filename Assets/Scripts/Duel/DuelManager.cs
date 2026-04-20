using System;
using System.Collections.Generic;
using Unity.Netcode;

public class DuelManager : NetworkBehaviour {
    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitialization;
    public event EventHandler<PlayersInitializedEventArgs> OnPlayersInitializationFinished;
    public event EventHandler<NextPlayerTurnEventArgs> OnNextPlayerTurn;
    public event EventHandler<NextFullTurnEventArgs> OnNextFullTurn;

    private List<MatchPlayer> players;
    private MatchPlayer localClientPlayer;
    private int currentPlayerTurnIndex;
    private int fullTurnCount;

    private void Awake() {
        currentPlayerTurnIndex = 0;
        fullTurnCount = 1;
    }

    private void Start() {
        GameManager.Instance.OnGameStart += InitializePlayers;
        EventBus.OnCreatureCardSelectedForPlay += PlayCreatureCardFromHand;
        EventBus.OnDomainCardSelectedForPlay += PlayDomainCardFromHand;
        EventBus.OnSpellCardSelectedForPlay += PlaySpellCardFromHand;
    }

    public void InitializePlayers(object sender, StartGameEventArgs args) {
        players = new List<MatchPlayer>();
        for(int i = 0; i < args.PlayerOrder.Count; i++) {
            MatchPlayer player;
            if (args.PlayerOrder[i] == NetworkManager.Singleton.LocalClientId) {
                player = new MatchPlayer(args.PlayerOrder[i], Temp_PopulateDeck());
                localClientPlayer = player;
            }
            else 
                player = new MatchPlayer(args.PlayerOrder[i], Temp_PopulateDeckNull());
            players.Add(player);
        }
        if (localClientPlayer == null)
            throw new Exception("Local Client Id not found in start game player list");

        OnPlayersInitialization?.Invoke(this, new PlayersInitializedEventArgs(players.Count, GetPlayerIndex(localClientPlayer)));
        OnPlayersInitializationFinished?.Invoke(this, new PlayersInitializedEventArgs(players.Count, GetPlayerIndex(localClientPlayer)));
    }

    private List<Card> Temp_PopulateDeck() {
        List<Card> result = new List<Card>();
        int tempDeckSize = 40;
        int databaseCardCount = CardDatabase.Instance.Cards.Count;
        for (int i = 0; i < tempDeckSize; i++) {
            Card card = CardDatabase.Instance.GetCardByIndex(UnityEngine.Random.Range(0, databaseCardCount)).GenerateCardFromBase();
            result.Add(card);
        }
        return result;
    }

    private List<Card> Temp_PopulateDeckNull() {
        List<Card> result = new List<Card>();
        int tempDeckSize = 40;
        int databaseCardCount = CardDatabase.Instance.Cards.Count;
        for (int i = 0; i < tempDeckSize; i++)
            result.Add(new NullCard());
        return result;
    }

    public void PlayCardFromHand(MatchPlayer player, int handIndex) {
        if (player.Hand.Count <= handIndex)
            throw new Exception("Attmepting to play card with invalid hadnIndex: " + handIndex);

        player.Hand[handIndex].PlayCardFromHand(player, handIndex);
    }

    public void PlayCreatureCardFromHand(object sender, PlayCardFromHandEventArgs<CreatureCard> args) {
        PlayCreatureCardFromHandServerRpc(GetPlayerIndex(args.Player.PlayerId), args.Card.GetNetworkSerializableObject(), args.HandIndex);
    }

    [Rpc(SendTo.Server)]
    private void PlayCreatureCardFromHandServerRpc(int playerIndex, CreatureCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        PlayCreatureCardFromHandClientRpc(playerIndex, cardNetworkSerializableObject, handIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayCreatureCardFromHandClientRpc(int playerIndex, CreatureCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        MatchPlayer player = Players[playerIndex];
        CreatureCard card = new CreatureCard(cardNetworkSerializableObject);
        card.CreatureHealthChangedCallback = player.OnCreatureHealthChangedCallback;
        card.CreatureDamagedCallback = player.OnCreatureDamagedCallback;
        card.CreatureDestroyedCallback = player.OnCreatureDestroyCallback;
        player.PlayCreatureCardFromHand(card, handIndex);
    }

    public void PlayDomainCardFromHand(object sender, PlayCardFromHandEventArgs<DomainCard> args) {
        PlayDomainCardFromHandServerRpc(GetPlayerIndex(args.Player.PlayerId), args.Card.GetNetworkSerializableObject(), args.HandIndex);
    }

    [Rpc(SendTo.Server)]
    private void PlayDomainCardFromHandServerRpc(int playerIndex, DomainCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        PlayDomainCardFromHandClientRpc(playerIndex, cardNetworkSerializableObject, handIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlayDomainCardFromHandClientRpc(int playerIndex, DomainCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        MatchPlayer player = Players[playerIndex];
        DomainCard card = new DomainCard(cardNetworkSerializableObject);
        player.PlayDomainCardFromHand(card, handIndex);
    }

    public void PlaySpellCardFromHand(object sender, PlayCardFromHandEventArgs<SpellCard> args) {
        PlaySpellCardFromHandServerRpc(GetPlayerIndex(args.Player.PlayerId), args.Card.GetNetworkSerializableObject(), args.HandIndex);
    }

    [Rpc(SendTo.Server)]
    private void PlaySpellCardFromHandServerRpc(int playerIndex, SpellCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        PlaySpellCardFromHandClientRpc(playerIndex, cardNetworkSerializableObject, handIndex);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlaySpellCardFromHandClientRpc(int playerIndex, SpellCardNetworkSerializable cardNetworkSerializableObject, int handIndex) {
        MatchPlayer player = Players[playerIndex];
        SpellCard card = new SpellCard(cardNetworkSerializableObject);
        player.PlaySpellCardFromHand(card, handIndex);
    }

    public void PlaySpellCard(object sender, PlayCardFromHandEventArgs<SpellCard> args) {
        if (args.Card.SpellType == SpellType.Instant)
            ExecuteSpellServerRpc(GetPlayerIndex(args.Player), args.Card.GetNetworkSerializableObject());
        else
            EventBus.InvokeOnActionChainSpellCardPlayed(this, new PlayerCardEventArgs<SpellCard>(args.Player, args.Card));
    }

    [Rpc(SendTo.Server)]
    public void ExecuteSpellServerRpc(int playerIndex, SpellCardNetworkSerializable cardNetworkSerializableObject) {
        ExecuteSpellClientRpc(playerIndex, cardNetworkSerializableObject);
    }

    [Rpc(SendTo.ClientsAndHost)]
    public void ExecuteSpellClientRpc(int playerIndex, SpellCardNetworkSerializable cardNetworkSerializableObject) {
        MatchPlayer player = Players[playerIndex];
        SpellCard spellCard = new SpellCard(cardNetworkSerializableObject);
        for (int i = 0; i < spellCard.BaseEffects.Count; i++) {
            spellCard.BaseEffects[i].Execute();
            // TODO: Execute the additional effects on the SpellCard class
        }
    }

    public void EndOfTurnRegenerateCreaturesHealth() {
        foreach(MatchPlayer player in Players) {
            for(int i = 0; i < player.Creatures.Count; i++) {
                if (player.Creatures[i].CurrentDamage > 0)
                    player.Creatures[i].CurrentDamage = 0;
            }
        }
    }

    public void NextTurn() {
        currentPlayerTurnIndex = ++currentPlayerTurnIndex % players.Count;
        OnNextPlayerTurn?.Invoke(this, new NextPlayerTurnEventArgs(GetCurrentPlayerTurn(), currentPlayerTurnIndex));
        if (currentPlayerTurnIndex == 0) {
            fullTurnCount++;
            OnNextFullTurn?.Invoke(this, new NextFullTurnEventArgs(fullTurnCount));
        }
    }

    public int GetPlayerCount() {
        return players.Count;
    }

    public MatchPlayer GetCurrentPlayerTurn() {
        return players[currentPlayerTurnIndex];
    }

    public bool IsLocalClientPlayerTurn() {
        return localClientPlayer == GetCurrentPlayerTurn();
    }

    public int GetStartOfTurnManaCount() {
        return fullTurnCount;
    }

    public MatchPlayer GetPlayerById(ulong playerId) {
        MatchPlayer result = null;
        foreach(MatchPlayer p in players) {
            if (p.PlayerId == playerId)
                result = p;
        }

        return result;
    }

    public void IncrementFullTurnCount() {
        fullTurnCount++;
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

    public List<MatchPlayer> Players { get { return players; } }

    public MatchPlayer LocalClientPlayer { get { return localClientPlayer; } }

    public int FullTurnCount { get { return fullTurnCount; } }
}
