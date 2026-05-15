using System;
using UnityEngine;

public class SpellChainUIController : MonoBehaviour {
    [SerializeField] private SpellChainManager spellChainManager;
    [SerializeField] private SpellChainUI spellChainUI;

    private void Start() {
        spellChainManager.OnSpellAddedToSpellChain += AddSpellToChain;
        spellChainManager.OnSpellRemovedFromSpellChain += RemoveSpellFromChain;
        spellChainManager.OnSpellChainFinished += ClearSpellChain;
    }

    private void AddSpellToChain(object sender, SpellCardAction action) {
        spellChainUI.AddSpellToChain(action.Card);
    }

    private void RemoveSpellFromChain(object sender, SpellCardAction action) {
        spellChainUI.RemoveSpellFromChain(action.Card.Uuid);
    }

    private void ClearSpellChain(object sender, EventArgs args) {
        TcgLogger.Log("ClearSpellChain Entered");
        spellChainUI.ClearSpellChain();
    }
}