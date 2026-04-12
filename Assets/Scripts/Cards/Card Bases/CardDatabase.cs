using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour {
    public static CardDatabase Instance { get; private set; }

    [SerializeField] private List<CreatureCardBase> creatureCards;
    [SerializeField] private List<SpellCardBase> spellCards;
    [SerializeField] private List<DomainCardBase> domainCards;
    
    private List<CardBase> cards;

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("CardDatabase already exists in scene. Destroying redundant object.");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        cards = new List<CardBase>();
        cards.AddRange(creatureCards);
        cards.AddRange(spellCards);
        cards.AddRange(domainCards);
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

    public CreatureCardBase GetCreatureCardByIndex(int index) {
        if (index > cards.Count)
            throw new Exception("Card index Out of Bounds for index: " + index);

        return creatureCards[index];
    }

    public CreatureCardBase GetCreatureCardByName(string name) {
        if (name == null)
            throw new Exception("Search by card name cannot be null");

        for (int i = 0; i < creatureCards.Count; i++) {
            if (name.ToLower() == creatureCards[i].CardName.ToLower())
                return creatureCards[i];
        }

        throw new Exception("Unable to find card with name: " + name);
    }

    public int GetIndexOf(CreatureCardBase creatureCard) {
        for(int i = 0; i < creatureCards.Count; i++) {
            if (creatureCards[i] == creatureCard)
                return i;
        }

        throw new Exception("Index of card not found");
    }

    public SpellCardBase GetSpellCardByIndex(int index) {
        if (index > spellCards.Count)
            throw new Exception("Card index Out of Bounds for index: " + index);

        return spellCards[index];
    }

    public SpellCardBase GetSpellCardByName(string name) {
        if (name == null)
            throw new Exception("Search by card name cannot be null");

        for (int i = 0; i < spellCards.Count; i++) {
            if (name.ToLower() == spellCards[i].CardName.ToLower())
                return spellCards[i];
        }

        throw new Exception("Unable to find card with name: " + name);
    }

    public int GetIndexOf(SpellCardBase spellCard) {
        for (int i = 0; i < spellCards.Count; i++) {
            if (spellCards[i] == spellCard)
                return i;
        }

        throw new Exception("Index of card not found");
    }

    public DomainCardBase GetDomainCardByIndex(int index) {
        if (index > domainCards.Count)
            throw new Exception("Card index Out of Bounds for index: " + index);

        return domainCards[index];
    }

    public DomainCardBase GetDomainCardByName(string name) {
        if (name == null)
            throw new Exception("Search by card name cannot be null");

        for (int i = 0; i < domainCards.Count; i++) {
            if (name.ToLower() == domainCards[i].CardName.ToLower())
                return domainCards[i];
        }

        throw new Exception("Unable to find card with name: " + name);
    }

    public int GetIndexOf(DomainCardBase spellCard) {
        for (int i = 0; i < domainCards.Count; i++) {
            if (domainCards[i] == spellCard)
                return i;
        }

        throw new Exception("Index of card not found");
    }

    public List<CardBase> Cards { get { return cards; } }

    public List<CreatureCardBase> CreatureCards { get { return creatureCards; } }

    public List<SpellCardBase> SpellCards { get { return spellCards; } }

    public List<DomainCardBase> DomainCards { get { return domainCards; } }
}
