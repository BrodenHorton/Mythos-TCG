using System;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour {
    public static CardDatabase Instance { get; private set; }

    [SerializeField] private CreatureCardBases creatureBases;
    [SerializeField] private SpellCardBases spellBases;
    [SerializeField] private DomainCardBases domainBases;
    
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
        cards.AddRange(creatureBases.Cards);
        cards.AddRange(spellBases.Cards);
        cards.AddRange(domainBases.Cards);
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

        return creatureBases.Cards[index];
    }

    public CreatureCardBase GetCreatureCardByName(string name) {
        if (name == null)
            throw new Exception("Search by card name cannot be null");

        for (int i = 0; i < creatureBases.Cards.Count; i++) {
            if (name.ToLower() == creatureBases.Cards[i].CardName.ToLower())
                return creatureBases.Cards[i];
        }

        throw new Exception("Unable to find card with name: " + name);
    }

    public int GetIndexOf(CreatureCardBase creatureCard) {
        for(int i = 0; i < creatureBases.Cards.Count; i++) {
            if (creatureBases.Cards[i] == creatureCard)
                return i;
        }

        throw new Exception("Index of card not found");
    }

    public SpellCardBase GetSpellCardByIndex(int index) {
        if (index > spellBases.Cards.Count)
            throw new Exception("Card index Out of Bounds for index: " + index);

        return spellBases.Cards[index];
    }

    public SpellCardBase GetSpellCardByName(string name) {
        if (name == null)
            throw new Exception("Search by card name cannot be null");

        for (int i = 0; i < spellBases.Cards.Count; i++) {
            if (name.ToLower() == spellBases.Cards[i].CardName.ToLower())
                return spellBases.Cards[i];
        }

        throw new Exception("Unable to find card with name: " + name);
    }

    public int GetIndexOf(SpellCardBase spellCard) {
        for (int i = 0; i < spellBases.Cards.Count; i++) {
            if (spellBases.Cards[i] == spellCard)
                return i;
        }

        throw new Exception("Index of card not found");
    }

    public DomainCardBase GetDomainCardByIndex(int index) {
        if (index > domainBases.Cards.Count)
            throw new Exception("Card index Out of Bounds for index: " + index);

        return domainBases.Cards[index];
    }

    public DomainCardBase GetDomainCardByName(string name) {
        if (name == null)
            throw new Exception("Search by card name cannot be null");

        for (int i = 0; i < domainBases.Cards.Count; i++) {
            if (name.ToLower() == domainBases.Cards[i].CardName.ToLower())
                return domainBases.Cards[i];
        }

        throw new Exception("Unable to find card with name: " + name);
    }

    public int GetIndexOf(DomainCardBase spellCard) {
        for (int i = 0; i < domainBases.Cards.Count; i++) {
            if (domainBases.Cards[i] == spellCard)
                return i;
        }

        throw new Exception("Index of card not found");
    }

    public List<CardBase> Cards { get { return cards; } }

    public List<CreatureCardBase> CreatureCards { get { return creatureBases.Cards; } }

    public List<SpellCardBase> SpellCards { get { return spellBases.Cards; } }

    public List<DomainCardBase> DomainCards { get { return domainBases.Cards; } }
}
