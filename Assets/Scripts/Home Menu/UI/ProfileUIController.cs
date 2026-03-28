using UnityEngine;

public class ProfileUIController : MonoBehaviour {
    [SerializeField] private PlayerProfile playerProfile;
    [SerializeField] private ProfileUI profileUI;

    private void Start() {
        LobbyUIController lobbyUIController = FindFirstObjectByType<LobbyUIController>();
        if (lobbyUIController != null) {
            lobbyUIController.OnLobbyUIOpened += (sender, args) => {
                profileUI.gameObject.SetActive(false);
            };
        }
        HomeMenuUIController homeMenuUIController = FindFirstObjectByType<HomeMenuUIController>();
        if (homeMenuUIController != null)
            homeMenuUIController.ProfileBtn.onClick.AddListener(OpenProfileUI);

        profileUI.UsernameInputField.onEndEdit.AddListener(UpdatePlayerProfileUsername);
    }

    public void OpenProfileUI() {
        profileUI.gameObject.SetActive(true);
        profileUI.UpdateProfileUI(playerProfile);
    }

    private void UpdatePlayerProfileUsername(string username) {
        if(username == null || username.Trim() == "") {
            profileUI.UsernameInputField.text = playerProfile.Username;
            return;
        }

        playerProfile.Username = username;
    }
}
