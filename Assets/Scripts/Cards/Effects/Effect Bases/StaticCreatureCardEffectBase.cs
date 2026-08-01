using System;
using UnityEngine;

public abstract class StaticCreatureCardEffectBase : CreatureCardEffectBase {
    [SerializeField] private string effectIconId;

    public string EffectIconId { get { return effectIconId; } }
}
