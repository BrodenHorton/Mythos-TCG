using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionChainUI : MonoBehaviour {
    [SerializeField] private Transform actionChainOrigin;
    [SerializeField] private float actionSpacing;
    [Header("Prefabs")]
    [SerializeField] private SpellActionUI spellActionUIPrefab;

    private List<SpellActionUI> actionChain;

    private void Awake() {
        actionChain = new List<SpellActionUI>();
    }

    public void AddAction(SpellCard card) {
        SpellActionUI actionUI = Instantiate(spellActionUIPrefab);
        actionUI.transform.position = spellActionUIPrefab.transform.position;
        actionUI.Init(card);
        actionChain.Add(actionUI);
        SpaceActions();
    }

    public void RemoveAction(Guid cardUuid) {
        for(int i = 0; i < actionChain.Count; i++) {
            if (actionChain[i].CardUuid == cardUuid) {
                actionChain.RemoveAt(i);
                SpaceActions();
                return;
            }
        }

        throw new Exception("Attempted to remove action UI that is not in the action chain");
    }

    private void SpaceActions() {
        int actionCount = actionChain.Count;
        float startingOffset = (actionCount - 1) * actionSpacing / 2;
        for (int i = 0; i < actionCount; i++) {
            SpellActionUI cardUI = actionChain[i];
            cardUI.transform.localScale = Vector3.one;
            cardUI.transform.eulerAngles = Vector3.zero;
            Vector3 cardPosition = actionChainOrigin.position;
            cardPosition.x += startingOffset - (i * actionSpacing);
            cardUI.transform.position = cardPosition;
        }
    }
}

public class ActionChainUIController : MonoBehaviour {
    [SerializeField] private ActionChainUI actionChainUI;
    [SerializeField] private ActionChainManager actionChainManager;

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