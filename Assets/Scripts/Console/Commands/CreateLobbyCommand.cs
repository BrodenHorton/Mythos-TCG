
public class CreateLobbyCommand : Command {
    private TcgLobby tcgLobby;

    public CreateLobbyCommand(TcgLobby tcgLobby) {
        cmdName = "createLobby";
        this.tcgLobby = tcgLobby;
    }

    public override void Execute(string[] args) {
        tcgLobby.CreateLobby();
    }
}
