using System;
using UnityEngine;

public class SpellChainUIController : MonoBehaviour {
    [SerializeField] private SpellChainUI spellChainUI;
    
    private SpellChainManager spellChainManager;

    private void Start() {
        spellChainManager = ServiceLocator.Get<SpellChainManager>();

        spellChainManager.OnSpellAddedToSpellChain += AddSpellToChain;
        spellChainManager.OnSpellRemovedFromSpellChain += RemoveSpellFromChain;
        spellChainManager.OnSpellChainFinished += ClearSpellChain;
    }

    private void AddSpellToChain(object sender, PlayerCardPayloadEventArgs<SpellCardPayload> args) {
        spellChainUI.AddSpellToChain(args.CardPayload);
    }

    private void RemoveSpellFromChain(object sender, PlayerCardPayloadEventArgs<SpellCardPayload> args) {
        spellChainUI.RemoveSpellFromChain(args.CardPayload.Uuid);
    }

    private void ClearSpellChain(object sender, EventArgs args) {
        spellChainUI.ClearSpellChain();
    }
}