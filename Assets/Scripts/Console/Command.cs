using UnityEngine;

public abstract class Command : ScriptableObject {
    [SerializeField] protected string cmdName;
    [SerializeField, TextArea] protected string description;

    public abstract void Execute(string[] args);

    public string Name { get { return cmdName; } }

    public string Description { get { return description; } }
}

[CreateAssetMenu(fileName = "CreateLobbyCommand", menuName = "Scriptable Objects/Command/Create Lobby")]
public class CreateLobbyCommand : Command {
    [SerializeField] private TCGLobby tcgLobby;

    public override void Execute(string[] args) {
        tcgLobby.CreateLobby();
    }
}