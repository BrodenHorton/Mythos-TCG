using System;
using System.Collections.Generic;
using UnityEngine;

public class BattlefieldUI : MonoBehaviour {
    [SerializeField] private List<PlayingFieldUI> playingFieldUIs = new List<PlayingFieldUI>();

    private void Awake() {
        if (playingFieldUIs.Count < 2)
            throw new Exception("Must have two or more PlayingFieldUIs");
    }

}
