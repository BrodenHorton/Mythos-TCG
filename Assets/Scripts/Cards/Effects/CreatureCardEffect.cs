using UnityEngine;

[System.Serializable]
public abstract class CreatureCardEffect {
    protected CreatureCard card;

    public CreatureCardEffect() {}

    public void Init(CreatureCard card) {
        this.card = card;
    }

    public abstract void AddListener();

    public abstract void RemoveListener();

    public abstract void Execute();
}
