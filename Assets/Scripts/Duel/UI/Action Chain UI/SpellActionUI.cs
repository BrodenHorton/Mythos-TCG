using System;
using UnityEngine;

public class SpellActionUI : MonoBehaviour {
    private SpriteRenderer spriteRenderer;
    private Guid cardUuid;

    public void Init(SpellCard spellCard) {
        spriteRenderer.sprite = spellCard.SplashArt;
        cardUuid = spellCard.Uuid;
    }

    public Guid CardUuid { get { return cardUuid; } }
}