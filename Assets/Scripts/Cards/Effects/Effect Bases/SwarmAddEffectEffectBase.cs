using UnityEngine;

[CreateAssetMenu(fileName = "Swarm Add Effect Effect Base", menuName = "Scriptable Objects/Effect/Base/Swarm Add Effect")]
public class SwarmAddEffectEffectBase : CreatureCardEffectBase {
    [SerializeField] private CreatureCardEffectBase addedEffectBase;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new SwarmAddEffectEffect(this);
    }

    public CreatureCardEffectBase AddedEffectBase { get { return addedEffectBase; } }
}