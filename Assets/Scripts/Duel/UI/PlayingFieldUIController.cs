using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class PlayingFieldUIController : NetworkBehaviour {
    [SerializeField] private PlayingFieldUI playingFieldUI;

    private MatchPlayer player;
    private DuelManager duelManager;
    private DuelStateManager stateManager;
    private Camera cam;
    private PlayerInputActions playerInputActions;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
    }

    private void Start() {
        EventBus.OnCreatureCardPlayedFromHand += PlayCreatureCard;
        EventBus.OnDomainCardPlayed += PlayDomainCard;
        EventBus.OnCreatureTapped += TapCreature;
        EventBus.OnCreatureUntapped += UntapCreature;
        EventBus.OnDeclareAttacker += RemoveAttacker;
        EventBus.OnUndeclareAttacker += UndeclareAttacker;
        EventBus.OnReleaseCombatCreatures += GetCreatureCardsFromCombat;
    }

    public void Init(MatchPlayer player) {
        this.player = player;
    }

    public void PlayCreatureCard(object sender, PlayCreatureCardFromHandEventArgs args) {
        if (player.PlayerId != args.Player.PlayerId)
            return;

        playingFieldUI.PlayCreatureCard(args.Card);
    }

    public void UndeclareAttacker(object sender, UndeclareAttackerEventArgs args) {
        if (player.PlayerId != args.Initiator.PlayerId)
            return;

        playingFieldUI.PlayCreatureCard(args.Attacker);
    }

    public void PlayDomainCard(object sender, PlayerSpellCardEventArgs args) {
        if (player.PlayerId != args.Player.PlayerId)
            return;

        playingFieldUI.PlayDomainCard(args.Card);
    }

    public void TapCreature(object sender, CreatureCardEventArgs args) {
        playingFieldUI.TapCreature(args.Card);
    }

    public void UntapCreature(object sender, CreatureCardEventArgs args) {
        playingFieldUI.UntapCreature(args.Card);
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (!duelManager.IsLocalClientPlayerTurn())
            return;
        if (player != duelManager.LocalClientPlayer)
            return;
        if (stateManager.CurrentState != stateManager.CombatPhase)
            return;
        CreatureFieldCardUI cardUI = RaycastColliderCheck();
        if (cardUI == null)
            return;
        if (!playingFieldUI.ContainsCreature(cardUI))
            return;
        if (!player.ContainsCreatureUuid(cardUI.CardUuid))
            return;
        CreatureCard creatureCard = player.GetCreatureByUuid(cardUI.CardUuid);
        if (creatureCard == null)
            return;
        if (!creatureCard.CanAttack())
            return;

        MatchPlayer initiator = duelManager.GetCurrentPlayerTurn();
        MatchPlayer target = duelManager.Players[(duelManager.GetPlayerIndex(initiator.PlayerId) + 1) % duelManager.Players.Count];
        DeclareAttackerServerRpc(duelManager.GetPlayerIndex(initiator), duelManager.GetPlayerIndex(target), creatureCard.Uuid.ToString());
    }

    private CreatureFieldCardUI RaycastColliderCheck() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        CreatureFieldCardUI fieldCardUI = null;
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<CreatureFieldCardCollisionPointer>()) {
                fieldCardUI = hit.collider.GetComponent<CreatureFieldCardCollisionPointer>().FieldCardUI;
                break;
            }
        }

        return fieldCardUI;
    }

    [Rpc(SendTo.Server)]
    private void DeclareAttackerServerRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        DeclareAttackerClientRpc(initiatorIndex, targetIndex, cardUuidStr);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void DeclareAttackerClientRpc(int initiatorIndex, int targetIndex, FixedString128Bytes cardUuidStr) {
        Guid cardUuid = Guid.Parse(cardUuidStr.ToString());
        CreatureCard creatureCard = duelManager.Players[initiatorIndex].GetCreatureByUuid(cardUuid);
        EventBus.InvokeOnDelcareAttacker(this, new DeclareAttackerEventArgs(duelManager.Players[initiatorIndex], duelManager.Players[targetIndex], creatureCard));
    }

    private void RemoveAttacker(object sender, DeclareAttackerEventArgs args) {
        if (args.Initiator.PlayerId != player.PlayerId)
            return;
        if (!playingFieldUI.ContainsCreature(args.Attacker.Uuid))
            return;

        playingFieldUI.RemoveCreature(args.Attacker.Uuid);
    }

    private void GetCreatureCardsFromCombat(object sender, ReleaseCombatCreaturesEventArgs args) {
        if (player != args.Player)
            return;

        for (int i = 0; i < args.Creatures.Count; i++)
            playingFieldUI.AddCreatureFieldCard(args.Creatures[i]);
    }

    public bool ContainsCreature(CreatureFieldCardUI creatureFieldCardUI) {
        return playingFieldUI.ContainsCreature(creatureFieldCardUI);
    }

    public PlayingFieldUI PlayingFieldUI { get { return playingFieldUI; } }
}