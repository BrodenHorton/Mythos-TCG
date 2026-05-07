using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(DuelManager))]
public class DuelStateManager : NetworkBehaviour {
    [SerializeField] private InitializationPhase initializationPhase;
    [SerializeField] private UntapPhase untapPhase;
    [SerializeField] private DrawPhase drawPhase;
    [SerializeField] private FirstMainPhase firstMainPhase;
    [SerializeField] private CombatPhase combatPhase;
    [SerializeField] private SecondMainPhase secondMainPhase;
    [SerializeField] private EndPhase endPhase;
    
    private DuelManager duelManager;
    private DuelState currentState;

    private void Awake() {
        currentState = initializationPhase;
    }

    private void Start() {
        if (!IsServer)
            return;
        
        duelManager = GetComponent<DuelManager>();
        if (duelManager == null)
            throw new Exception("DuelManager not found on GameObject");

        DuelManager.OnPlayersInitializationFinished += StartStateMachine;
    }

    private void Update() {
        if (!IsServer)
            return;
        if (GameManager.Instance.GameState != GameState.Duel)
            return;

        currentState.UpdateState();
    }

    public void StartStateMachine(object sender, EventArgs args) {
        if (!IsServer)
            return;

        currentState.EnterState();
    }

    public void SwitchState(DuelState state) {
        currentState = state;
        currentState.EnterState();
    }

    public DuelManager DuelManager { get { return duelManager; } }

    public DuelState CurrentState { get { return currentState; } }

    public InitializationPhase Initialization { get { return initializationPhase; } }

    public UntapPhase UntapPhase { get { return untapPhase; } }

    public DrawPhase DrawPhase { get { return drawPhase; } }

    public FirstMainPhase FirstMainPhase { get { return firstMainPhase; } }

    public CombatPhase CombatPhase { get { return combatPhase; } }

    public SecondMainPhase SecondMainPhase { get { return secondMainPhase; } }

    public EndPhase EndPhase { get { return  endPhase; } }
}
