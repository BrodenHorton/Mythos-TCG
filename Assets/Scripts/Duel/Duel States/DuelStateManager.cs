using System;
using UnityEngine;

[RequireComponent(typeof(DuelManager))]
public class DuelStateManager : MonoBehaviour {
    private UntapPhase untapPhase;
    private DrawPhase drawPhase;
    private MainPhase mainPhase;
    private CombatPhase combatPhase;
    private EndPhase endPhase;
    private DuelState currentState;

    private DuelManager duelManager;
    private bool isFirstUpdate = true;

    private void Awake() {
        duelManager = GetComponent<DuelManager>();
        if (duelManager == null)
            throw new Exception("DuelManager not found on GameObject");
        CombatManager combatManager = FindFirstObjectByType<CombatManager>();
        if(combatManager == null)
            throw new Exception("Unable to find GameObject with CombatManager component");

        untapPhase = new UntapPhase(this);
        drawPhase = new DrawPhase(this);
        mainPhase = new MainPhase(this);
        combatPhase = new CombatPhase(this, combatManager);
        endPhase = new EndPhase(this);

        currentState = untapPhase;
    }

    private void Update() {
        if(isFirstUpdate) {
            currentState.EnterState();
            isFirstUpdate = false;
        }

        currentState.UpdateState();
    }

    public void SwitchState(DuelState state) {
        currentState = state;
        currentState.EnterState();
    }

    public DuelManager DuelManager { get { return duelManager; } }

    public DuelState CurrentState { get { return currentState; } }

    public UntapPhase UntapPhase { get { return untapPhase; } }

    public DrawPhase DrawPhase { get { return drawPhase; } }

    public MainPhase MainPhase { get { return mainPhase; } }

    public CombatPhase CombatPhase { get { return combatPhase; } }

    public EndPhase EndPhase { get { return  endPhase; } }
}
