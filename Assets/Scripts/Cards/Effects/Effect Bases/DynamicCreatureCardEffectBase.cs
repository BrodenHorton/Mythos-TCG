using UnityEngine;

public abstract class DynamicCreatureCardEffectBase : CreatureCardEffectBase {
    [SerializeField] private string dynamicEffectType;

    public string DynamicEffectType { get { return dynamicEffectType; } }
}