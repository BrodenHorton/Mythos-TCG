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

    public CardBase GetCardById(string id) {
        if (id == null)
            throw new Exception("Cannot search for card base with null Id");

        for(int i = 0; i < cards.Count; i++) {
            if (cards[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return cards[i];
        }

        throw new Exception("Unable to find card base with Id: " + id);
    }

    public CreatureCardBase GetCreatureCardById(string id) {
        if (id == null)
            throw new Exception("Cannot search for card base with null id");

        for (int i = 0; i < creatureBases.Cards.Count; i++) {
            if (creatureBases.Cards[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return creatureBases.Cards[i];
        }

        throw new Exception("Unable to find card with Id: " + id);
    }

    public SpellCardBase GetSpellCardById(string id) {
        if (id == null)
            throw new Exception("Cannot search for card base with null id");

        for (int i = 0; i < spellBases.Cards.Count; i++) {
            if (spellBases.Cards[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return spellBases.Cards[i];
        }

        throw new Exception("Unable to find card with Id: " + id);
    }

    public DomainCardBase GetDomainCardById(string id) {
        if (id == null)
            throw new Exception("Cannot search for card base with null id");

        for (int i = 0; i < domainBases.Cards.Count; i++) {
            if (domainBases.Cards[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return domainBases.Cards[i];
        }

        throw new Exception("Unable to find card with Id: " + id);
    }

    public List<CardBase> Cards { get { return cards; } }

    public List<CreatureCardBase> CreatureCards { get { return creatureBases.Cards; } }

    public List<SpellCardBase> SpellCards { get { return spellBases.Cards; } }

    public List<DomainCardBase> DomainCards { get { return domainBases.Cards; } }
}
