using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayingFieldUIController : MonoBehaviour {
    [SerializeField] private PlayingFieldUI playingFieldUI;

    private MatchPlayer player;
    private DuelManager duelManager;
    private Camera cam;
    private PlayerInputActions playerInputActions;

    private void Awake() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");

        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
    }

    private void Start() {
        EventBus.OnCreatureCardPlayed += PlayCreatureCard;
        EventBus.OnDomainCardPlayed += PlayDomainCard;
        EventBus.OnCreatureTapped += TapCreature;
        EventBus.OnCreatureUntapped += UntapCreature;
    }

    public void Init(MatchPlayer player) {
        this.player = player;
    }

    public void PlayCreatureCard(object sender, PlayerCreatureCardEventArgs args) {
        if (player.Uuid != args.Player.Uuid)
            return;

        playingFieldUI.PlayCreatureCard(args.Card);
    }

    public void PlayDomainCard(object sender, PlayerSpellCardEventArgs args) {
        if (player.Uuid != args.Player.Uuid)
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
        if (player.Uuid != duelManager.GetCurrentPlayerTurn().Uuid)
            return;
        CreatureFieldCardUI fieldCardUI = RaycastColliderCheck();
        if (fieldCardUI == null)
            return;
        if (!playingFieldUI.ContainsCreatureCard(fieldCardUI))
            return;
        if (!player.ContainsCreatureUuid(fieldCardUI.CardUuid))
            return;
        CreatureCard creatureCard = player.GetCreatureByUuid(fieldCardUI.CardUuid);
        if (creatureCard == null)
            return;
        if (!creatureCard.CanAttack())
            return;

        Debug.Log("Creature Card tapped");
        creatureCard.Tap();
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
}