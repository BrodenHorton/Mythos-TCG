using System;
using TMPro;
using UnityEngine;

public abstract class HandCardUI : MonoBehaviour {
    [SerializeField] protected GameObject selectableBorder;
    [SerializeField] protected TextMeshProUGUI cardName;
    [SerializeField] protected TextMeshProUGUI manaCost;
    [SerializeField] protected RectTransform infoContainer;
    [SerializeField] protected RectTransform uniqueEffectContainer;
    [SerializeField] protected TextMeshProUGUI uniqueEffectTextPrefab;

    protected Guid cardUuid;
    protected bool isSelectable;

    private void Awake() {
        selectableBorder.SetActive(false);
        isSelectable = false;
    }

    public void SetSelectable(bool isSelectable) {
        selectableBorder.SetActive(isSelectable);
        this.isSelectable = isSelectable;
    }

    public Guid CardUuid { get { return cardUuid; } }

    public bool IsSelectable { get { return isSelectable; } }
}
