using UnityEngine;

public abstract class BlessingEffect : CreatureCardEffect {
    private static string EFFECT_PREFIX = "<color=#fafa64>Blessing</color>";

    [SerializeField] protected string dynamicEffectName;

    protected abstract void BlessingEffectHandler(object sender, LifePointsChangedEventArgs args);

    public override string GetFullDescription() {
        return EFFECT_PREFIX + ": " + description;
    }
}
