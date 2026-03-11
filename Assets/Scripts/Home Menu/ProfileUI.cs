using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUI : MonoBehaviour {
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TextMeshProUGUI gems;
    [SerializeField] private TextMeshProUGUI gamesPlayed;
    [SerializeField] private TextMeshProUGUI winCount;
    [SerializeField] private TextMeshProUGUI highestWinStreak;
    [SerializeField] private Button closeBtn;

    private void Awake() {
        closeBtn.onClick.AddListener(() => {
            gameObject.SetActive(false);
        });
    }

    private void Start() {
        gameObject.SetActive(false);
    }

    public void UpdateProfileUI(PlayerProfile playerProfile) {
        usernameInputField.text = playerProfile.Username;
        gems.text = playerProfile.GemsCount.ToString();
        gamesPlayed.text = playerProfile.GamesPlayedCount.ToString();
        winCount.text = playerProfile.WinCount.ToString();
        highestWinStreak.text = playerProfile.HighestWinStreak.ToString();
    }

    public TMP_InputField UsernameInputField { get { return usernameInputField; } }
}