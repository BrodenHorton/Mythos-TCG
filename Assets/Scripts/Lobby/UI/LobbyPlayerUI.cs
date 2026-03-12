using TMPro;
using UnityEngine;

public class LobbyPlayerUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI readyText;

    private string playerId;

    private void Start() {
        readyText.gameObject.SetActive(false);
    }

    public void SetUsername(string username) {
        usernameText.text = username;
    }

    public void SetPlayerReady(bool isReady) {
        if(!readyText.gameObject.activeSelf)
            readyText.gameObject.SetActive(true);
    }

    public string PlayerId { get { return playerId; } set { playerId = value; } }
}
