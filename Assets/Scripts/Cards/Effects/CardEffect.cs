using UnityEngine;

public abstract class CardEffect {

    public CardEffect() {

    }

    public abstract void AddListener();
    public abstract void RemoveListener();
    public abstract void Execute();
}
