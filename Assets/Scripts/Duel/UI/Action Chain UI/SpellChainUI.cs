using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellChainUI : MonoBehaviour {
    [SerializeField] private Transform spellChainOrigin;
    [SerializeField] private float spellActionSpacing;
    [Header("Prefabs")]
    [SerializeField] private SpellActionUI spellActionUIPrefab;

    private List<SpellActionUI> spellChain;

    private void Awake() {
        spellChain = new List<SpellActionUI>();
    }

    private void Start() {
        
    }

    public void AddAction(SpellCard card) {
        SpellActionUI actionUI = Instantiate(spellActionUIPrefab);
        actionUI.transform.position = spellActionUIPrefab.transform.position;
        actionUI.Init(card);
        spellChain.Add(actionUI);
        SpaceActions();
    }

    public void RemoveAction(Guid cardUuid) {
        for(int i = 0; i < spellChain.Count; i++) {
            if (spellChain[i].CardUuid == cardUuid) {
                spellChain.RemoveAt(i);
                SpaceActions();
                return;
            }
        }

        throw new Exception("Attempted to remove action UI that is not in the action chain");
    }

    private void SpaceActions() {
        int actionCount = spellChain.Count;
        float startingOffset = (actionCount - 1) * spellActionSpacing / 2;
        for (int i = 0; i < actionCount; i++) {
            SpellActionUI cardUI = spellChain[i];
            cardUI.transform.localScale = Vector3.one;
            cardUI.transform.eulerAngles = Vector3.zero;
            Vector3 cardPosition = spellChainOrigin.position;
            cardPosition.x += startingOffset - (i * spellActionSpacing);
            cardUI.transform.position = cardPosition;
        }
    }
}

public class SpellChainUIController : MonoBehaviour {
    [SerializeField] private SpellChainManager spellChainManager;
    [SerializeField] private SpellChainUI spellChainUI;

    private void Start() {
        
    }


}