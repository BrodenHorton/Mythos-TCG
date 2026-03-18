using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour {
    public static CardDatabase Instance { get; private set; }

    [SerializeField] private List<CardBase> cards;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("CardDatabase already exists in scene. Destroying redundant object.");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public CardBase GetCardByIndex(int index) {
        if (index > cards.Count)
            throw new Exception("Card index Out of Bounds for index: " + index);

        return cards[index];
    }

    public CardBase GetCardByName(string name) {
        if (name == null)
            throw new Exception("Search by card name cannot be null");

        for(int i = 0; i < cards.Count; i++) {
            if(name.ToLower() == cards[i].CardName.ToLower())
                return cards[i];
        }

        throw new Exception("Unable to find card with name: " + name);
    }

    public List<CardBase> Cards { get { return cards; } }
}
