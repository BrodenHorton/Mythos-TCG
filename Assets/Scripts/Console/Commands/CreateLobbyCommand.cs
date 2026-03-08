using UnityEngine;

[CreateAssetMenu(fileName = "CreateLobbyCommand", menuName = "Scriptable Objects/Command/Create Lobby")]
public class CreateLobbyCommand : Command {
    [SerializeField] private TCGLobby tcgLobby;

    public override void Execute(string[] args) {
        tcgLobby.CreateLobby();
    }
}