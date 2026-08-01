using System;
using UnityEngine;

[Serializable]
public abstract class CreatureCardEffectBase : ScriptableObject {
    [SerializeField] private string effectName;

    public abstract CreatureCardEffect CreateCreatureCardEffect();

    public abstract string GetFullDescription();

    public string EffectName { get { return effectName; } }
}
