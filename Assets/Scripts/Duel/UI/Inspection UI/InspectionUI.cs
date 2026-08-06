using System;
using UnityEngine;

public class InspectionUI : MonoBehaviour {
    [SerializeField] private GameObject background;
    [SerializeField] private CreatureInspectionCardUI creatureInspectionCardUI;
    [SerializeField] private DomainInspectionCardUI domainInspectionCardUI;
    [SerializeField] private SpellInspectionCardUI spellInspectionCardUI;

    private bool isOpen;

    private void Awake() {
        isOpen = false;
    }

    private void Start() {
        Hide();
    }

    public void Inspect(CardPayload card) {
        if (isOpen)
            throw new Exception("Attempting to inspect a card while the inspection UI is already open");

        isOpen = true;
        if (card is CreatureCardPayload creatureCardPayload)
            creatureInspectionCardUI.UpdateUI(creatureCardPayload);
        else if (card is DomainCardPayload domainCardPayload)
            domainInspectionCardUI.UpdateUI(domainCardPayload);
        else if (card is SpellCardPayload spellCardPayload)
            spellInspectionCardUI.UpdateUI(spellCardPayload);
        else
            throw new Exception("Unrecognized card payload");
    }

    public void Hide() {
        isOpen = false;
        background.SetActive(false);
        creatureInspectionCardUI.gameObject.SetActive(false);
        domainInspectionCardUI.gameObject.SetActive(false);
        spellInspectionCardUI.gameObject.SetActive(false);
    }

    public bool IsOpen { get { return isOpen; } }
}
