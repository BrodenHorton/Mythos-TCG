using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI usernameText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private Button readyBtn;

    private string playerId;

    public void SetUsername(string username) {
        usernameText.text = username;
    }

    public void SetPlayerReady(bool isReady) {
        if(readyBtn.gameObject.activeSelf)
            readyBtn.gameObject.SetActive(false);
        if(!readyText.gameObject.activeSelf)
            readyText.gameObject.SetActive(true);
    }

    public string PlayerId { get { return playerId; } set { playerId = value; } }
}
