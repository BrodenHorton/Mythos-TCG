using System.Collections.Generic;
using System;
using UnityEngine;

public class CardEffectRegistry : MonoBehaviour {
    [SerializeField] private List<CreatureCardEffectBase> creatureCardEffectBases;

    private void Awake() {
        ServiceLocator.Register(this);
    }

    private void OnDestroy() {
        ServiceLocator.Unregister(this);
    }

    public CreatureCardEffectBase GetCreatureEffectBaseById(string id) {
        for (int i = 0; i < creatureCardEffectBases.Count; i++) {
            if (creatureCardEffectBases[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return creatureCardEffectBases[i];
        }
        throw new Exception("Unable to find card effect with id: " + id);
    }

    public bool ContainsCreatureEffectBase(string id) {
        for (int i = 0; i < creatureCardEffectBases.Count; i++) {
            if (creatureCardEffectBases[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}
