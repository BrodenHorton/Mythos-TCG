using System;
using Unity.Netcode;

[Serializable]
public abstract class Card : INetworkSerializable {
    protected Guid uuid;
    protected CardType cardType;

    public Card() {
        uuid = Guid.NewGuid();
    }

    public abstract bool IsPlayable(DuelManager duelManager, DuelStateManager stateMaager, SpellChainManager spellChainManager, MatchPlayer player);

    public abstract void PlayCard(MatchPlayer player);

    public abstract void PlayCardFromHand(MatchPlayer player);

    public abstract int GetManaCost();

    public abstract void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;

    public Guid Uuid { get { return uuid; } }

    public CardType CardType { get { return cardType; } }
}
