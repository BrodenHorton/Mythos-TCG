
public class JoinLobbyCommand : Command {
    private TcgLobby tcgLobby;

    public JoinLobbyCommand(TcgLobby tcgLobby) {
        cmdName = "joinLobby";
        this.tcgLobby = tcgLobby;
    }

    public override void Execute(string[] args) {
        if(args.Length != 1) {
            TcgLogger.Log("&aInvalid Arguments");
            return;
        }
        
        tcgLobby.JoinLobbyByCode(args[0]);
    }
}
