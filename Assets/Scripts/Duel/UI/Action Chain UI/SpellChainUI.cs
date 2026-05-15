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

    public void AddSpellToChain(SpellCard card) {
        SpellActionUI actionUI = Instantiate(spellActionUIPrefab);
        actionUI.transform.parent = spellChainOrigin;
        actionUI.Init(card);
        spellChain.Add(actionUI);
        SpaceActions();
    }

    public void RemoveSpellFromChain(Guid cardUuid) {
        for(int i = 0; i < spellChain.Count; i++) {
            if (spellChain[i].CardUuid == cardUuid) {
                SpellActionUI spellActionUI = spellChain[i];
                spellChain.RemoveAt(i);
                Destroy(spellActionUI.gameObject);
                SpaceActions();
                return;
            }
        }

        throw new Exception("Attempted to remove action UI that is not in the action chain");
    }

    public void ClearSpellChain() {
        for (int i = spellChain.Count - 1; i >= 0; i--) {
            SpellActionUI spellActionUI = spellChain[i];
            spellChain.RemoveAt(i);
            Destroy(spellActionUI.gameObject);
        }
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
