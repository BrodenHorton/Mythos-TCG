using System.Collections.Generic;
using UnityEngine;

public class CombatFieldUI : MonoBehaviour {
    [SerializeField] private Transform attackerOrigin;
    [SerializeField] private Transform defenderOrigin;
    [SerializeField] private List<CreatureFieldCardUI> attackers;
    [SerializeField] private List<CreatureFieldCardUI> defenders;
    [Header("Prefab")]
    [SerializeField] private CreatureFieldCardUI creatureFieldCardUIPrefab;

    private float cardSpacing = 0.6f;

    public void AddAttacker(CreatureCard card) {
        CreatureFieldCardUI cardUI = Instantiate(creatureFieldCardUIPrefab);
        cardUI.Init(card);
        attackers.Add(cardUI);
        SpaceAttackers();
    }

    // TODO: Implement so defenders face corresponding attackers
    public void AddDefender(CreatureCard card) {
        CreatureFieldCardUI cardUI = Instantiate(creatureFieldCardUIPrefab);
        cardUI.Init(card);
        defenders.Add(cardUI);
    }

    public void RemoveCreature(CreatureFieldCardUI cardUI) {
        attackers.Remove(cardUI);
        Destroy(cardUI.gameObject);
        SpaceAttackers();
    }

    public bool ContainsAttacker(CreatureFieldCardUI other) {
        foreach (CreatureFieldCardUI cardUI in attackers) {
            if (cardUI == other)
                return true;
        }

        return false;
    }

    private void SpaceAttackers() {
        int cardCount = attackers.Count;
        float handOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            Vector3 cardPosition = attackerOrigin.position;
            cardPosition.x += i * cardSpacing - handOffset;
            attackers[i].transform.position = cardPosition;
        }
    }
}
