public interface IEffectSequence<TCard> {
    void Init(TCard card);

    void RemoveListeners();
}
#endregion
