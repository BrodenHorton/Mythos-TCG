using System;
using UnityEngine;

[RequireComponent(typeof(DuelManager))]
public class DuelStateManager : MonoBehaviour {
    private DrawPhase drawPhase;
    private StartPhase startPhase;
    private MainPhase mainPhase;
    private BattlePhase battlePhase;
    private EndPhase endPhase;
    private DuelState currentState;

    private DuelManager duelManager;
    private bool isFirstUpdate = true;

    private void Awake() {
        duelManager = GetComponent<DuelManager>();
        if (duelManager == null)
            throw new Exception("DuelManager not found on GameObject");

        drawPhase = new DrawPhase(this);
        startPhase = new StartPhase(this);
        mainPhase = new MainPhase(this);
        battlePhase = new BattlePhase(this);
        endPhase = new EndPhase(this);

        currentState = drawPhase;
    }

    private void Update() {
        if(isFirstUpdate) {
            currentState.EnterState();
            isFirstUpdate = false;
        }
    }

    public void SwitchState(DuelState state) {
        currentState = state;
        currentState.EnterState();
    }

    public DuelManager DuelManager { get { return duelManager; } }

    public DrawPhase DrawPhase { get { return drawPhase; } }

    public StartPhase StartPhase { get { return startPhase; } }

    public MainPhase MainPhase { get { return mainPhase; } }

    public BattlePhase BattlePhase { get { return battlePhase; } }

    public EndPhase EndPhase { get { return  endPhase; } }
}
