using TMPro;
using UnityEngine;

public abstract class HandCardUI : CardUI {
    [SerializeField] protected TextMeshProUGUI cardName;
    [SerializeField] protected TextMeshProUGUI manaCost;
    [SerializeField] protected RectTransform infoContainer;
    [SerializeField] protected RectTransform uniqueEffectContainer;
    [SerializeField] protected TextMeshProUGUI uniqueEffectTextPrefab;
}
