public class PlayerEventArgs {
    private ulong playerId;

    public PlayerEventArgs(ulong playerId) {
        this.playerId = playerId;
    }

    public ulong PlayerId { get { return playerId; } }
}
