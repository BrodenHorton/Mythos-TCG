public class ListLobbiesCommand : Command {
    private TcgLobby tcgLobby;

    public ListLobbiesCommand(TcgLobby tcgLobby) {
        cmdName = "listLobbies";
        this.tcgLobby = tcgLobby;
    }

    public override void Execute(string[] args) {
        TcgLogger.Log("List Lobbies");
        tcgLobby.ListLobbies();
    }
}