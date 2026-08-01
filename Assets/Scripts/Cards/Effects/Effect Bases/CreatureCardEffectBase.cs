using UnityEngine;

public abstract class CreatureCardEffectBase : ScriptableObject {
    [SerializeField] private string effectName;
    [SerializeField] private string description;

    public abstract CreatureCardEffect CreateCreatureCardEffect();

    public string EffectName { get { return effectName; } }

    public string Description { get { return description; } }
}

[CreateAssetMenu(fileName = "Bloodthirsty Effect Base", menuName = "Scriptable Objects/Effect/Base/Bloodthirsty")]
public class BloodthirstyEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new BloodthirstyEffect(this);
    }
}

[CreateAssetMenu(fileName = "Lifelink Effect Base", menuName = "Scriptable Objects/Effect/Base/Life Link")]
public class LifelinkEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new LifelinkEffect(this);
    }
}

[CreateAssetMenu(fileName = "Deathtouch Effect Base", menuName = "Scriptable Objects/Effect/Base/Deathtouch")]
public class DeathtouchEffectBase : StaticCreatureCardEffectBase {

    public override CreatureCardEffect CreateCreatureCardEffect() {
        return new DeathtouchEffect(this);
    }
}