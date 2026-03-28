using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeMenuUI : MonoBehaviour {
    [SerializeField] private Button profileBtn;
    [SerializeField] private TextMeshProUGUI gemsLabel;
    [SerializeField] private TextMeshProUGUI gemsCount;

    public Button ProfileBtn { get { return profileBtn; } }
}
