using System;
using UnityEngine;

[Serializable]
public class StatBoostEffectData {
    [SerializeField] private int atkBoost;
    [SerializeField] private int healthBoost;
    [SerializeField] private bool isResetAfterTurn;

    public int AtkBoost { get { return atkBoost; } }

    public int HealthBoost { get { return healthBoost; } }

    public bool IsResetAfterTurn { get { return isResetAfterTurn; } }
}