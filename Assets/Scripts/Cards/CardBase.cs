using UnityEngine;
using UnityEngine.UIElements;

public abstract class CardBase : ScriptableObject {
    [SerializeField] protected string cardName;
    [SerializeField] Image splashArt;
    [SerializeField] protected int manaCost;
    [SerializeField] protected Domain domain;

    public abstract Card GenerateCardFromBase();

    public string CardName { get { return cardName; } }

    public Image SplashArt { get { return splashArt; } }

    public int ManaCost { get { return manaCost; } }

    public Domain Domain { get { return domain; } }
}
