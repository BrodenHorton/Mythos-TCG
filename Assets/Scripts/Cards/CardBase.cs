using UnityEngine;

public abstract class CardBase : ScriptableObject {
    [SerializeField] protected string id;
    [SerializeField] protected string cardName;
    [SerializeField] Material splashArt;
    [SerializeField] protected int manaCost;
    [SerializeField] protected Domain domain;

    public abstract Card GenerateCardFromBase();

    public string Id { get { return id; } }

    public string CardName { get { return cardName; } }

    public Material SplashArt { get { return splashArt; } }

    public int ManaCost { get { return manaCost; } }

    public Domain Domain { get { return domain; } }
}
