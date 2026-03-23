using TMPro;
using UnityEngine;

public class GameStateUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI duelPhase;
    [SerializeField] private TextMeshProUGUI playerTurnIndex;
    [SerializeField] private TextMeshProUGUI fullTurn;

    public void SetDuelPhase(string duelPhaseText) {
        duelPhase.text = duelPhaseText;
    }

    public void SetPlayerTurnIndex(int playerTurnIndex) {
        this.playerTurnIndex.text = playerTurnIndex.ToString();
    }

    public void SetFullTurn(int fullTurn) {
        this.fullTurn.text = fullTurn.ToString();
    }
}
