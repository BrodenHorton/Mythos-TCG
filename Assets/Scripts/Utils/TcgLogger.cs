using System;
using UnityEngine;

public class TcgLogger : MonoBehaviour {
    public event EventHandler<string> OnLog;

    public static TcgLogger Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("TcgLogger already exists in scene. Destroying redundant object.");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        Application.logMessageReceived += LogConsoleException;
    }

    public static void Log(string msg) {
        if (Instance == null)
            throw new Exception("TcgLogger Instance is null");

        Debug.Log(msg);
        Instance.OnLog?.Invoke(Instance, msg);
    }

    public static void Log(TcgLogSender sender, string msg) {
        if (Instance == null)
            throw new Exception("TcgLogger Instance is null");

        Debug.Log(msg);
        Instance.OnLog?.Invoke(Instance, sender.GetLogPrefix() + " " + msg);
    }

    private void LogConsoleException(string exceptionMessage, string stackTrace, LogType logType) {
        if (logType != LogType.Exception && logType != LogType.Error)
            return;

        Log("&1" + exceptionMessage + "\n" + stackTrace);
    }
}
