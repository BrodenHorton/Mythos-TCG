using System;

[Serializable]
public class PlayerCardEventArgs<T> : PlayerEventArgs where T : Card {
    private T card;

    public PlayerCardEventArgs(ulong playerId, T card) : base(playerId) {
        this.card = card;
    }

    public T Card { get { return card; } }
}
