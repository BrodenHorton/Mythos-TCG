using UnityEngine;

public class LifeGainEffectData {
    [SerializeField] private int lifePointsModifier;

    public int LifePointsModifier { get { return lifePointsModifier; } }
}