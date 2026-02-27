using System;
using UnityEngine;

[RequireComponent(typeof(DuelManager))]
public class DuelStateManager : MonoBehaviour {
    private UntapPhase untapPhase;
    private DrawPhase drawPhase;
    private FirstMainPhase firstMainPhase;
    private CombatPhase combatPhase;
    private SecondMainPhase secondMainPhase;
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
        firstMainPhase = new FirstMainPhase(this);
        combatPhase = new CombatPhase(this, combatManager);
        secondMainPhase = new SecondMainPhase(this);
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

    public FirstMainPhase FirstMainPhase { get { return firstMainPhase; } }

    public CombatPhase CombatPhase { get { return combatPhase; } }

    public SecondMainPhase SecondMainPhase { get { return secondMainPhase; } }

    public EndPhase EndPhase { get { return  endPhase; } }
}
