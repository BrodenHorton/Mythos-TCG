using UnityEngine;
using UnityEngine.UI;

public class ProfileUIController : MonoBehaviour {
    [SerializeField] private PlayerProfile playerProfile;
    [SerializeField] private Button btn;
    [SerializeField] private ProfileUI profileUI;

    private void Awake() {
        btn.onClick.AddListener(OpenProfileUI);
    }

    private void Start() {
        profileUI.UsernameInputField.onEndEdit.AddListener((s) => { });
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
