using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CombatFieldUI : MonoBehaviour {
    protected static readonly int MAX_FIELD_CREATURES = 6;

    [SerializeField] protected Transform attackerOrigin;
    [SerializeField] protected Transform defenderOrigin;
    [SerializeField] protected float cardSpacing;
    [SerializeField] protected Vector2 combatFieldForwardVector;
    [SerializeField] protected Vector2 spacingVector;
    [Header("Prefab")]
    [SerializeField] protected CreatureFieldCardUI creatureFieldCardUIPrefab;

    protected ulong targetPlayerId;
    protected Dictionary<int, CreatureFieldCardUI> attackerByPositionIndex;
    protected Dictionary<int, CreatureFieldCardUI> defenderByPositionIndex;

    protected virtual void Awake() {
        attackerByPositionIndex = new Dictionary<int, CreatureFieldCardUI>();
        defenderByPositionIndex = new Dictionary<int, CreatureFieldCardUI>();
    }

    public void Init(ulong targetPlayerId) {
        this.targetPlayerId = targetPlayerId;
    }

    public void AddAttacker(CreatureCard card) {
        if (attackerByPositionIndex.Count >= MAX_FIELD_CREATURES)
            throw new Exception("Attempting to add new attacker when there is already " + MAX_FIELD_CREATURES + " attackers");

        CreatureFieldCardUI cardUI = Instantiate(creatureFieldCardUIPrefab);
        cardUI.Init(card);
        for (int i = 0; i < MAX_FIELD_CREATURES; i++) {
            if (!attackerByPositionIndex.ContainsKey(i)) {
                attackerByPositionIndex.Add(i, cardUI);
                break;
            }
        }
        SpaceAttackers();
        TcgLogger.Log("Added Attacker to Combat Field");
    }

    public void AddDefender(CreatureCard defender, Guid attackerCardUuid) {
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

        CreatureFieldCardUI cardUI = Instantiate(creatureFieldCardUIPrefab);
        cardUI.Init(defender);
        defenderByPositionIndex.Add(attackerIndex, cardUI);
        PlaceDefender(cardUI, attackerIndex);
    }

    public void RemoveAttacker(Guid uuid) {
        foreach (KeyValuePair<int, CreatureFieldCardUI> entry in attackerByPositionIndex.ToList()) {
            if (entry.Value.CardUuid == uuid) {
                CreatureFieldCardUI cardUI = entry.Value;
                attackerByPositionIndex.Remove(entry.Key);
                Destroy(cardUI.gameObject);
                SpaceAttackers();
                break;
            }
        }
    }

    public void RemoveDefender(Guid uuid) {
        foreach (KeyValuePair<int, CreatureFieldCardUI> entry in defenderByPositionIndex.ToList()) {
            if (entry.Value.CardUuid == uuid) {
                CreatureFieldCardUI cardUI = entry.Value;
                defenderByPositionIndex.Remove(entry.Key);
                Destroy(cardUI.gameObject);
                break;
            }
        }
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

    protected void SpaceAttackers() {
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

    protected void PlaceDefender(CreatureFieldCardUI cardUI, int attackerIndex) {
        cardUI.transform.position = defenderOrigin.position;
        Vector3 positionDiff = attackerByPositionIndex[attackerIndex].transform.position - defenderOrigin.position;
        cardUI.transform.position = new Vector3(
            cardUI.transform.position.x + (positionDiff.x * spacingVector.x),
            cardUI.transform.position.y,
            cardUI.transform.position.z + (positionDiff.z * spacingVector.y));
    }

    public void ClearCreatures() {
        attackerByPositionIndex.Clear();
        defenderByPositionIndex.Clear();
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

    public ulong TargetPlayerId { get { return targetPlayerId; } }

    public List<CreatureFieldCardUI> Attackers { get { return attackerByPositionIndex.Values.ToList(); } }

    public List<CreatureFieldCardUI> Defenders { get { return defenderByPositionIndex.Values.ToList(); } }
}