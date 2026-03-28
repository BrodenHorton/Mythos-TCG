using UnityEngine;
using UnityEngine.UI;

public class HomeMenuUIController : MonoBehaviour {
    [SerializeField] private HomeMenuUI homeMenuUI;

    private void Start() {
        LobbyUIController lobbyUIController = FindFirstObjectByType<LobbyUIController>();
        if (lobbyUIController != null) {
            lobbyUIController.OnLobbyUIOpened += (sender, args) => {
                homeMenuUI.gameObject.SetActive(false);
            };
        }
    }

    public Button ProfileBtn { get { return homeMenuUI.ProfileBtn; } }
}