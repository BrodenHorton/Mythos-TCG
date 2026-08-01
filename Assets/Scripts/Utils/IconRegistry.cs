using System.Collections.Generic;
using System;
using UnityEngine;

public class IconRegistry : MonoBehaviour {
    [SerializeField] private List<StaticKeywordIcon> staticKeywordIcons;

    private void Awake() {
        ServiceLocator.Register(this);
    }

    private void OnDestroy() {
        ServiceLocator.Unregister(this);
    }

    public Sprite Get(string id) {
        for (int i = 0; i < staticKeywordIcons.Count; i++) {
            if (staticKeywordIcons[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return staticKeywordIcons[i].Icon;
        }
        throw new Exception("Unable to find static keyword with id: " + id);
    }

    public bool Contains(string id) {
        for (int i = 0; i < staticKeywordIcons.Count; i++) {
            if (staticKeywordIcons[i].Id.Equals(id, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    public List<StaticKeywordIcon> StaticKeywordIcons { get { return staticKeywordIcons; } }
}
