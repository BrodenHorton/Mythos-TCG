using UnityEngine;

public class CreatureCardEffect : CardEffect {
    private CreatureCardBase card; // Change to card instance

    public CreatureCardEffect(CreatureCardBase card) : base() {
        this.card = card;
    }

    public override void AddListener()
    {
        throw new System.NotImplementedException();
    }

    public override void RemoveListener()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }
}
