using System.Collections.Generic;
using System;
using UnityEngine;

public class DynamicKeywordRegistry : MonoBehaviour {
    [SerializeField] private List<DynamicKeyword> keywords;

    private void Awake() {
        ServiceLocator.Register(this);
    }

    private void OnDestroy() {
        ServiceLocator.Unregister(this);
    }

    public DynamicKeyword Get(string id) {
        for (int i = 0; i < keywords.Count; i++) {
            if (keywords[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return keywords[i];
        }
        throw new Exception("Unable to find dynamic keyword with id: " + id);
    }

    public bool Contains(string id) {
        for (int i = 0; i < keywords.Count; i++) {
            if (keywords[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }
}