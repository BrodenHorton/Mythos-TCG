using UnityEngine;

public class CreateLobbyCommand : Command {
    private TCGLobby tcgLobby;

    public CreateLobbyCommand(TCGLobby tcgLobby) {
        cmdName = "createLobby";
        this.tcgLobby = tcgLobby;
    }

    public override void Execute(string[] args) {
        tcgLobby.CreateLobby();
    }
}