using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MatchPlayer {
    [SerializeField] private List<Card> cards;
    [SerializeField] private int seriesWinCount;
}
