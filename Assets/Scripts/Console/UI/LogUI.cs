using TMPro;
using UnityEngine;

public class LogUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI logText;

    public void SetText(string text) {
        logText.text = text;
    }
}
