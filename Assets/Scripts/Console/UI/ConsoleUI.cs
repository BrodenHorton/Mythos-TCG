using UnityEngine;

public class ConsoleUI : MonoBehaviour {
    [SerializeField] private GameObject logContainer;
    [SerializeField] private ConsoleInputField consoleInputField;
    [Header("Prefabs")]
    [SerializeField] private LogUI logPrefab;

    void Start() {
        consoleInputField.IsActive = true;
    }

    void Update() {
        
    }
}
