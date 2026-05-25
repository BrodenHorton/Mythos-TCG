public class PlayerCardCancelableEventArgs<T> : PlayerCardEventArgs<T> where T : Card {
    private bool isCanceled;
    
    public PlayerCardCancelableEventArgs(ulong playerId, T card) : base(playerId, card) {
        isCanceled = false;
    }

    public bool IsCanceled { get { return isCanceled; } set { isCanceled = value; } }
}