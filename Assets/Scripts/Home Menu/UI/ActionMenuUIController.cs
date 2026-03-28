using UnityEngine;

public class ActionMenuUIController : MonoBehaviour {
    [SerializeField] private ActionMenuUI actionMenuUI;

    private void Start() {
        LobbyUIController lobbyUIController = FindFirstObjectByType<LobbyUIController>();
        if (lobbyUIController != null) {
            lobbyUIController.OnLobbyUIOpened += (sender, args) => {
                actionMenuUI.gameObject.SetActive(false);
            };
        }
    }
}