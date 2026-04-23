using System;
using UnityEngine;

public class ActionChainUIController : MonoBehaviour {
    [SerializeField] private SpellChainUI actionChainUI;
    [SerializeField] private SpellChainManager actionChainManager;

    private void Start() {
        actionChainManager.OnActionAddedToActionChain += AddAction;
    }

    public void AddAction(object sender, SpellCardAction action) {
        actionChainUI.AddAction(action.Card);
    }

    public void RemoveAction(Guid cardUuid) {
        actionChainUI.RemoveAction(cardUuid);
    }
}