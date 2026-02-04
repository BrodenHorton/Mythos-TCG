using UnityEngine;

public class DuelStateManager : MonoBehaviour {
    [SerializeField] private DuelManager duelManager; // Check if it is on the same Object

    private DrawPhase drawPhase;
    private StartPhase startPhase;
    private MainPhase mainPhase;
    private BattlePhase battlePhase;
    private EndPhase endPhase;
    private DuelState currentState;

    private void Awake() {
        drawPhase = new DrawPhase();
        startPhase = new StartPhase();
        mainPhase = new MainPhase();
        battlePhase = new BattlePhase();
        endPhase = new EndPhase();

        currentState = startPhase;
    }

    public void SwitchState(DuelState state) {
        currentState = state;
        currentState.EnterState(this);
    }

    public DuelManager DuelManager { get { return duelManager; } }

    public DrawPhase DrawPhase { get { return drawPhase; } }

    public StartPhase StartPhase { get { return startPhase; } }

    public MainPhase MainPhase { get { return mainPhase; } }

    public BattlePhase BattlePhase { get { return battlePhase; } }

    public EndPhase EndPhase { get { return  endPhase; } }
}
