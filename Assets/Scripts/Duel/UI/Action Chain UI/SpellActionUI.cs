using System;
using UnityEngine;

public class SpellActionUI : MonoBehaviour {
    [SerializeField] private MeshRenderer meshRenderer;
    private Guid cardUuid;

    public void Init(SpellCardPayload spellCard) {
        meshRenderer.material = spellCard.CardBase.SplashArt;
        cardUuid = spellCard.Uuid;
    }

    public Guid CardUuid { get { return cardUuid; } }
}