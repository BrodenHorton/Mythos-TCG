using UnityEngine;
using UnityEngine.UI;

public class ProfileUIController : MonoBehaviour {
    [SerializeField] private PlayerProfile playerProfile;
    [SerializeField] private Button openProfileBtn;
    [SerializeField] private ProfileUI profileUI;

    private void Awake() {
        openProfileBtn.onClick.AddListener(OpenProfileUI);
    }

    private void Start() {
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
