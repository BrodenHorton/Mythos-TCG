using System;
using System.Collections.Generic;
using UnityEngine;

public class DuelUIManager : MonoBehaviour {
    [SerializeField] private PlayerUIController playerUIController;
    [SerializeField] private List<OpponentUIController> opponentUIControllers;
    [SerializeField] private List<PlayingFieldUIController> playingFieldUIControllers;

    private void Awake() {
        DuelManager duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        List<MatchPlayer> players = duelManager.Players;
        if(players.Count != opponentUIControllers.Count + 1)
            throw new Exception("Number of Match Players and UIControllers do not match. " + 
                players.Count + " Match Players and " + opponentUIControllers.Count + 1 + " UIControllers");
        if (players.Count != playingFieldUIControllers.Count)
            throw new Exception("Number of Match Players and PlayingFieldUIs do not match. " +
                players.Count + " Match Players and " + playingFieldUIControllers.Count + " PlayingFieldUIs");

        playerUIController.Init(players[0]);
        for (int i = 0; i < opponentUIControllers.Count; i++)
            opponentUIControllers[i].Init(players[i + 1]);
        for(int i = 0; i < playingFieldUIControllers.Count; i++)
            playingFieldUIControllers[i].Init(players[i]);
    }
}
