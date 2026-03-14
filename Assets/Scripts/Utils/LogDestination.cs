using UnityEngine;

// Has to be an abstract class since it can't be set in the inspector unless it is a unity object
public abstract class LogDestination : MonoBehaviour {
    public abstract void AddLog(string msg);
}
