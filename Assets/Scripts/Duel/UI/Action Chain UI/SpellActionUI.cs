using System;
using UnityEngine;

public class SpellActionUI : MonoBehaviour {
    [SerializeField] private MeshRenderer meshRenderer;
    private Guid cardUuid;

    public void Init(SpellCard spellCard) {
        meshRenderer.material = spellCard.SplashArt;
        cardUuid = spellCard.Uuid;
    }

    public Guid CardUuid { get { return cardUuid; } }
}