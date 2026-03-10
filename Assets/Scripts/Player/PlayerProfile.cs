using System.Collections.Generic;
using UnityEngine;

public class PlayerProfile : MonoBehaviour {
    [SerializeField] private string username;
    [SerializeField] private List<CardBase> collection;
    [SerializeField] private List<Deck> decks;
    [SerializeField] private int gemsCount;
    [SerializeField] private int gamesPlayedCount;
    [SerializeField] private int winCount;
    [SerializeField] private int highestWinStreak;

    public string Username { get { return username; } set { username = value; } }

    public List<CardBase> Collection { get { return collection; } }

    public List<Deck> Decks { get { return decks; } }

    public int GemsCount { get { return gemsCount; } }

    public int GamesPlayedCount { get { return gamesPlayedCount; } }

    public int WinCount { get { return winCount; } }

    public int HighestWinStreak { get { return highestWinStreak; } }
}
