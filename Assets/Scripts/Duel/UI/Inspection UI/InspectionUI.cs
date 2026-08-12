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

    public void InspectCard(CardPayload card) {
        if (isOpen)
            throw new Exception("Attempting to inspect a card when the inspection UI is alrady open");

        isOpen = true;
        background.SetActive(true);
        if (card is CreatureCardPayload creatureCardPayload) {
            creatureInspectionCardUI.gameObject.SetActive(true);
            creatureInspectionCardUI.UpdateUI(creatureCardPayload);
        }
        /*else if (card is DomainCardPayload domainCardPayload)
            domainInspectionCardUI.UpdateUI(domainCardPayload);
        else if (card is SpellCardPayload spellCardPayload)
            spellInspectionCardUI.UpdateUI(spellCardPayload);*/
        else
            throw new Exception("Unrecognized card payload");
    }

    public void Hide() {
        isOpen = false;
        background.SetActive(false);
        creatureInspectionCardUI.ResetUI();
        creatureInspectionCardUI.gameObject.SetActive(false);
        /*domainInspectionCardUI.ResetUI();
        domainInspectionCardUI.gameObject.SetActive(false);
        spellInspectionCardUI.ResetUI();
        spellInspectionCardUI.gameObject.SetActive(false);*/
    }

    public bool IsOpen { get { return isOpen; } }
}
