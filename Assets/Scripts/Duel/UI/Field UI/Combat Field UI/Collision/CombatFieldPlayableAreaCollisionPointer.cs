using UnityEngine;

public class CombatFieldPlayableAreaCollisionPointer : MonoBehaviour {
    [SerializeField] private CombatFieldPlayableArea combatFieldPlayableArea;

    public CombatFieldPlayableArea CombatFieldPlayableArea { get { return combatFieldPlayableArea; } }
}