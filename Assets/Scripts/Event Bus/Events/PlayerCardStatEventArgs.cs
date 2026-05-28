public class PlayerCardStatEventArgs<T> : PlayerCardEventArgs<T> where T : Card {
    private int value;

    public PlayerCardStatEventArgs(ulong playerId, T card, int value) : base(playerId, card) {
        this.value = value;
    }

    public int Value { get { return value; } set { this.value = value; } }
}