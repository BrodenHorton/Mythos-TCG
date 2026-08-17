using UnityEngine;

[CreateAssetMenu(fileName = "Evolve Effect Base", menuName = "Scriptable Objects/Effect/Base/Evolve")]
public class EvolveEffectBase : CreatureCardEffectBase {
    [SerializeField] private CreatureCardBase evolution;
    [SerializeField] private int evolutionTurnCount;

    public override CreatureCardEffect GenerateCardEffectFromBase() {
        return new EvolveEffect(this);
    }

    public CreatureCardBase Evolution { get { return evolution; } }

    public int EvolutionTurnCount { get { return evolutionTurnCount; } }
}