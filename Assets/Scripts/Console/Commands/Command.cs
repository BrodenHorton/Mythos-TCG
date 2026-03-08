using UnityEngine;

public abstract class Command : ScriptableObject {
    [SerializeField] protected string cmdName;

    public abstract void Execute(string[] args);

    public string Name { get { return cmdName; } }
}
