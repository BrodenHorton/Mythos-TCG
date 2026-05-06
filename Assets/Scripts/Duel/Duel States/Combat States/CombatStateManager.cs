using System;
using Unity.Netcode;
using UnityEngine;

public class CombatStateManager : NetworkBehaviour {
    [SerializeField] private OutOfCombatState outOfCombatState;
    [SerializeField] private DeclareAttackersState declareAttackersState;
    [SerializeField] private DeclareDefendersState declareDefendersState;
    [SerializeField] private DeclareSpellsState declareSpellsState;
    [SerializeField] private ProcessCombatState processCombatState;

    private DuelManager duelManager;
    private CombatManager combatManager;
    private CombatState currentState;

    private void Awake() {
        currentState = outOfCombatState;
    }

    private void Start() {
        if (!IsServer)
            return;

        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Unable to find GameObject with DuelManager component");
        combatManager = FindFirstObjectByType<CombatManager>();
        if (combatManager == null)
            throw new Exception("Unable to find GameObject with CombatManager component");
    }

    private void Update() {
        if (!IsServer)
            return;

        currentState.UpdateState();
    }

    public void SwitchState(CombatState state) {
        if (!IsServer)
            return;

        currentState = state;
        currentState.EnterState();
    }

    public void StartCombat() {
        if (!IsServer)
            return;

        SwitchState(declareAttackersState);
    }

    public DuelManager DuelManager { get { return duelManager; } }

    public CombatManager CombatManager { get { return combatManager; } }

    public CombatState CurrentState { get { return currentState; } }

    public OutOfCombatState OutOfCombatState { get { return outOfCombatState; } }

    public DeclareAttackersState DeclareAttackersState { get { return declareAttackersState; } }

    public DeclareDefendersState DeclareDefendersState { get { return declareDefendersState; } }

    public DeclareSpellsState DeclareSpellsState { get { return declareSpellsState; } }

    public ProcessCombatState ProcessCombatState { get { return processCombatState; } }
}