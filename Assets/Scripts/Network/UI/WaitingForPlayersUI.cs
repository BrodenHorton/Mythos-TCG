using TMPro;
using UnityEngine;

public class WaitingForPlayersUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI connectedPlayers;
    [SerializeField] private TextMeshProUGUI maxPlayers;

    public void UpdateUI(int connectedPlayersCount, int maxPlayersCount) {
        connectedPlayers.text = connectedPlayersCount.ToString();
        maxPlayers.text = maxPlayersCount.ToString();
    }
}
