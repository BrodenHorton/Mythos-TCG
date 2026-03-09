using System;
using UnityEngine;

public class TcgLogger : MonoBehaviour {
    [SerializeField] private ConsoleUIController consoleUIController;

    public static TcgLogger Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("TcgLogger already exists in scene. Destroying redundant object.");
            Destroy(this);
        }
        else
            Instance = this;
    }

    public static void Log(string msg) {
        if (Instance == null)
            throw new Exception("TcgLogger Instance is null");

        Debug.Log(msg);
        Instance.consoleUIController.AddChatLog(msg);
    }

    public static void LogExclusive(string msg) {
        if (Instance == null)
            throw new Exception("TcgLogger Instance is null");

        Instance.consoleUIController.AddChatLog(msg);
    }

}
