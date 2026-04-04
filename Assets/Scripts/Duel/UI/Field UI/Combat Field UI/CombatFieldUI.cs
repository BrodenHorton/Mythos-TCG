using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatFieldUI : MonoBehaviour {
    private static readonly int MAX_FIELD_CREATURES = 6;

    public event EventHandler<CombatFieldCardSelectEventArgs> OnSelectFieldCard;

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
    private Camera cam;

    private void Awake() {
        attackerByPositionIndex = new Dictionary<int, CreatureFieldCardUI>();
        defenderByPositionIndex = new Dictionary<int, CreatureFieldCardUI>();

        PlayerInputActions playerInputActions = GameInputManager.Instance.PlayerInputActions;
        playerInputActions.Player.Select.started += SelectFieldCard;
    }

    private void Start() {
        cam = Camera.main;
    }

    public void Init(ulong targetPlayerId) {
        this.targetPlayerId = targetPlayerId;
    }

    public void AddAttacker(CreatureCard card) {
        if (attackerByPositionIndex.Count >= MAX_FIELD_CREATURES)
            throw new Exception("Attempting to add new attacker when there is already " + MAX_FIELD_CREATURES + " attackers");

        CreatureFieldCardUI cardUI = Instantiate(creatureFieldCardUIPrefab);
        cardUI.Init(card);
        for(int i = 0; i < MAX_FIELD_CREATURES; i++) {
            if (!attackerByPositionIndex.ContainsKey(i)) {
                attackerByPositionIndex.Add(i, cardUI);
                break;
            }
        }
        SpaceAttackers();
        TcgLogger.Log("Added Attacker to Combat Field");
    }

    public void AddDefender(CreatureCard defender, CreatureCard attacker) {
        if (defenderByPositionIndex.Count >= MAX_FIELD_CREATURES)
            throw new Exception("Attempting to add new defender when there is already " + MAX_FIELD_CREATURES + " defenders");

        int attackerIndex = -1;
        for (int i = 0; i < MAX_FIELD_CREATURES; i++) {
            if (attackerByPositionIndex.ContainsKey(i) && attackerByPositionIndex[i].CardUuid == attacker.Uuid) {
                attackerIndex = i;
                break;
            }
        }
        if (attackerIndex == -1)
            throw new Exception("Unable to find attacker with Uuid " + attacker.Uuid);
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

    private void SelectFieldCard(InputAction.CallbackContext context) {
        if (!context.started)
            return;
        CreatureFieldCardUI cardUI = CreatureFieldCardRaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!ContainsAttacker(cardUI) && !ContainsDefender(cardUI))
            return;

        OnSelectFieldCard?.Invoke(this, new CombatFieldCardSelectEventArgs(this, cardUI));
    }

    private CreatureFieldCardUI CreatureFieldCardRaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        CreatureFieldCardUI cardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CreatureFieldCardCollisionPointer>()) {
                cardUI = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>().CardUI;
                break;
            }
        }

        return cardUI;
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
        Vector3 positionDiff = attackerByPositionIndex[attackerIndex].transform.position - defenderOrigin.position;
        cardUI.transform.position = new Vector3(positionDiff.x * spacingVector.x, cardUI.transform.position.y, positionDiff.z * spacingVector.y);
    }

    public void ClearCreatures() {
        attackerByPositionIndex.Clear();
        defenderByPositionIndex.Clear();
    }

    public void FlattenAttackerIndices() {
        List<KeyValuePair<int, CreatureFieldCardUI>> attackers = attackerByPositionIndex.ToList();
        for(int i = 0; i < attackers.Count; i++) {
            for(int j = 0; j < attackers.Count - i - 1; j++) {
                if (attackers[j].Key > attackers[j + 1].Key) {
                    KeyValuePair<int, CreatureFieldCardUI> temp = attackers[j + 1];
                    attackers[j + 1] = attackers[j];
                    attackers[j + 1] = temp;
                }
            }
        }

        attackerByPositionIndex.Clear();
        for(int i = 0; i < attackers.Count; i++)
            attackerByPositionIndex.Add(i, attackers[i].Value);
    }

    public int GetHighestAttackerPositionIndex() {
        int highestPositionIndex = -1;
        foreach(int index in attackerByPositionIndex.Keys) {
            if(index > highestPositionIndex)
                highestPositionIndex = index;
        }

        return highestPositionIndex;
    }

    public ulong TargetPlayerId { get { return targetPlayerId; } }

    public List<CreatureFieldCardUI> Attackers { get { return attackerByPositionIndex.Values.ToList(); } }

    public List<CreatureFieldCardUI> Defenders { get { return defenderByPositionIndex.Values.ToList(); } }
}
