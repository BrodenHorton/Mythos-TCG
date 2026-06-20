using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatFieldUI : MonoBehaviour {
    private static readonly int MAX_FIELD_CREATURES = 6;

    [SerializeField] private Transform attackerOrigin;
    [SerializeField] private Transform defenderOrigin;
    [SerializeField] private float cardSpacing;
    [SerializeField] private Vector2 combatFieldForwardVector;
    [SerializeField] private Vector2 spacingVector;
    [Header("Prefab")]
    [SerializeField] private CreatureFieldCardUI creatureFieldCardUIPrefab;

    private ulong targetPlayerId;
    private Dictionary<int, CreatureFieldCardUI> attackerByPositionIndex;
    private Dictionary<int, CreatureFieldCardUI> defenderByPositionIndex;

    private void Awake() {
        attackerByPositionIndex = new Dictionary<int, CreatureFieldCardUI>();
        defenderByPositionIndex = new Dictionary<int, CreatureFieldCardUI>();
    }

    public void Init(ulong targetPlayerId) {
        this.targetPlayerId = targetPlayerId;
    }

    public void AddAttacker(CreatureFieldCardUI cardUI) {
        if (attackerByPositionIndex.Count >= MAX_FIELD_CREATURES)
            throw new Exception("Attempting to add new attacker when there is already " + MAX_FIELD_CREATURES + " attackers");

        for (int i = 0; i < MAX_FIELD_CREATURES; i++) {
            if (!attackerByPositionIndex.ContainsKey(i)) {
                attackerByPositionIndex.Add(i, cardUI);
                cardUI.IsInCombatField = true;
                break;
            }
        }
        SpaceAttackers();
    }

    public void AddDefender(CreatureFieldCardUI defender, Guid attackerCardUuid) {
        if (defenderByPositionIndex.Count >= MAX_FIELD_CREATURES)
            throw new Exception("Attempting to add new defender when there is already " + MAX_FIELD_CREATURES + " defenders");

        int attackerIndex = -1;
        for (int i = 0; i < MAX_FIELD_CREATURES; i++) {
            if (attackerByPositionIndex.ContainsKey(i) && attackerByPositionIndex[i].CardUuid == attackerCardUuid) {
                attackerIndex = i;
                break;
            }
        }
        if (attackerIndex == -1)
            throw new Exception("Unable to find attacker with Uuid " + attackerCardUuid);
        if (defenderByPositionIndex.ContainsKey(attackerIndex))
            throw new Exception("Attempting to add a defender to the position " + attackerIndex + " which already has an active defender");

        defenderByPositionIndex.Add(attackerIndex, defender);
        defender.IsInCombatField = true;
        PlaceDefender(defender, attackerIndex);
    }

    public void RemoveAttacker(Guid uuid) {
        foreach (KeyValuePair<int, CreatureFieldCardUI> entry in attackerByPositionIndex.ToList()) {
            if (entry.Value.CardUuid == uuid) {
                CreatureFieldCardUI cardUI = entry.Value;
                attackerByPositionIndex.Remove(entry.Key);
                SpaceAttackers();
                return;
            }
        }
        throw new Exception("Attempting to remove attacker that is not in combat field");
    }

    public void RemoveDefender(Guid uuid) {
        foreach (KeyValuePair<int, CreatureFieldCardUI> entry in defenderByPositionIndex.ToList()) {
            if (entry.Value.CardUuid == uuid) {
                CreatureFieldCardUI cardUI = entry.Value;
                defenderByPositionIndex.Remove(entry.Key);
                return;
            }
        }
        throw new Exception("Attempting to remove defender that is not in combat field");
    }

    public CreatureFieldCardUI ReleaseAttacker(Guid uuid) {
        foreach (KeyValuePair<int, CreatureFieldCardUI> entry in attackerByPositionIndex.ToList()) {
            if (entry.Value.CardUuid == uuid) {
                CreatureFieldCardUI cardUI = entry.Value;
                attackerByPositionIndex.Remove(entry.Key);
                return cardUI;
            }
        }
        throw new Exception("Attempting to release attacker that is not in combat field");
    }

    public CreatureFieldCardUI ReleaseDefender(Guid uuid) {
        foreach (KeyValuePair<int, CreatureFieldCardUI> entry in defenderByPositionIndex.ToList()) {
            if (entry.Value.CardUuid == uuid) {
                CreatureFieldCardUI cardUI = entry.Value;
                defenderByPositionIndex.Remove(entry.Key);
                return cardUI;
            }
        }
        throw new Exception("Attempting to release defender that is not in combat field");
    }

    private void SpaceAttackers() {
        FlattenAttackerIndices();
        int cardCount = attackerByPositionIndex.Count;
        float fieldOffset = (cardCount - 1) * cardSpacing / 2;
        for (int i = 0; i < cardCount; i++) {
            if (!attackerByPositionIndex.ContainsKey(i))
                continue;

            CreatureFieldCardUI cardUI = attackerByPositionIndex[i];
            Vector3 cardPosition = attackerOrigin.position;
            cardPosition.x += i * cardSpacing - fieldOffset;
            cardUI.transform.position = cardPosition;
        }
    }

    private void PlaceDefender(CreatureFieldCardUI cardUI, int attackerIndex) {
        cardUI.transform.position = defenderOrigin.position;
        Vector3 positionDiff = attackerByPositionIndex[attackerIndex].transform.position - defenderOrigin.position;
        cardUI.transform.position = new Vector3(
            cardUI.transform.position.x + (positionDiff.x * spacingVector.x),
            cardUI.transform.position.y,
            cardUI.transform.position.z + (positionDiff.z * spacingVector.y));
    }

    public void FlattenAttackerIndices() {
        List<KeyValuePair<int, CreatureFieldCardUI>> attackers = attackerByPositionIndex.ToList();
        for (int i = 0; i < attackers.Count; i++) {
            for (int j = 0; j < attackers.Count - i - 1; j++) {
                if (attackers[j].Key > attackers[j + 1].Key) {
                    KeyValuePair<int, CreatureFieldCardUI> temp = attackers[j + 1];
                    attackers[j + 1] = attackers[j];
                    attackers[j + 1] = temp;
                }
            }
        }

        attackerByPositionIndex.Clear();
        for (int i = 0; i < attackers.Count; i++)
            attackerByPositionIndex.Add(i, attackers[i].Value);
    }

    public void ClearAttackers() {
        attackerByPositionIndex.Clear();
    }

    public void ClearDefenders() {
        defenderByPositionIndex.Clear();
    }

    public void ClearCreatures() {
        attackerByPositionIndex.Clear();
        defenderByPositionIndex.Clear();
    }

    public bool ContainsAttacker(CreatureFieldCardUI cardUI) {
        return attackerByPositionIndex.ContainsValue(cardUI);
    }

    public bool ContainsAttacker(Guid cardUuid) {
        foreach (CreatureFieldCardUI cardUI in attackerByPositionIndex.Values) {
            if (cardUI.CardUuid == cardUuid)
                return true;
        }

        return false;
    }

    public bool ContainsDefender(CreatureFieldCardUI cardUI) {
        return defenderByPositionIndex.ContainsValue(cardUI);
    }

    public bool ContainsDefender(Guid cardUuid) {
        foreach (CreatureFieldCardUI cardUI in defenderByPositionIndex.Values) {
            if (cardUI.CardUuid == cardUuid)
                return true;
        }

        return false;
    }

    public CreatureFieldCardUI GetAttacker(Guid cardUuid) {
        foreach (CreatureFieldCardUI cardUI in attackerByPositionIndex.Values) {
            if (cardUI.CardUuid == cardUuid)
                return cardUI;
        }

        throw new Exception("Unable to find attacker with uuid: " + cardUuid);
    }

    public CreatureFieldCardUI GetDefender(Guid cardUuid) {
        foreach (CreatureFieldCardUI cardUI in defenderByPositionIndex.Values) {
            if (cardUI.CardUuid == cardUuid)
                return cardUI;
        }

        throw new Exception("Unable to find defender with uuid: " + cardUuid);
    }

    public ulong TargetPlayerId { get { return targetPlayerId; } }

    public List<CreatureFieldCardUI> Attackers { get { return attackerByPositionIndex.Values.ToList(); } }

    public List<CreatureFieldCardUI> Defenders { get { return defenderByPositionIndex.Values.ToList(); } }
}