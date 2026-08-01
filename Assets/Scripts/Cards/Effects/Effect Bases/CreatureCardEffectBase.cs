using System;
using UnityEngine;

[Serializable]
public abstract class CreatureCardEffectBase : ScriptableObject {
    [SerializeField] private string id;
    [SerializeField] private string effectName;

    public abstract CreatureCardEffect CreateCreatureCardEffect();

    public abstract string GetFullDescription();

    public string Id { get { return id; } }

    public string EffectName { get { return effectName; } }
}
