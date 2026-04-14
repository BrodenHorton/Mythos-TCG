using System;
using UnityEngine;

public class CombatStateManager : MonoBehaviour {
    [SerializeField] private DeclareAttackersState declareAttackersState;
    [SerializeField] private DeclareDefendersState declareDefendersState;
    [SerializeField] private DeclareSpellsState declareSpellsState;
    [SerializeField] private ProcessCombatState processCombatState;
    [SerializeField] private OutOfCombatState outOfCombatState;

    private DuelManager duelManager;
    private CombatManager combatManager;
    private CombatState currentState;

    private void Awake() {
        currentState = outOfCombatState;
    }

    private void Start() {
        duelManager = GetComponent<DuelManager>();
        if (duelManager == null)
            throw new Exception("DuelManager not found on GameObject");
        combatManager = FindFirstObjectByType<CombatManager>();
        if (combatManager == null)
            throw new Exception("Unable to find GameObject with CombatManager component");
    }

    private void Update() {
        currentState.UpdateState();
    }

    public void SwitchState(CombatState state) {
        currentState = state;
        currentState.EnterState();
    }

    public DuelManager DuelManager { get { return duelManager; } }

    public CombatManager CombatManager { get { return combatManager; } }

    public CombatState CurrentState { get { return currentState; } }

    public DeclareAttackersState DeclareAttackersState { get { return declareAttackersState; } }

    public DeclareDefendersState DeclareDefendersState { get { return declareDefendersState; } }

    public DeclareSpellsState DeclareSpellsState { get { return declareSpellsState; } }

    public ProcessCombatState ProcessCombatState { get { return processCombatState; } }

    public OutOfCombatState OutOfCombatState { get { return outOfCombatState; } }
}