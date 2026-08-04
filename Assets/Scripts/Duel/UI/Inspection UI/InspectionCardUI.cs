using TMPro;
using UnityEngine;

public abstract class InspectionCardUI<T> : MonoBehaviour where T : CardPayload  {
    [SerializeField] protected TextMeshProUGUI cardName;
    [SerializeField] protected TextMeshProUGUI manaCost;
    [SerializeField] protected RectTransform infoContainer;
    [SerializeField] protected RectTransform uniqueEffectContainer;
    [SerializeField] protected TextMeshProUGUI uniqueEffectTextPrefab;

    public abstract void UpdateUI(T cardPayload);

    public abstract void ClearUI();
}
