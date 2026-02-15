using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour {
    [SerializeField] private PlayerUI playerUI;

    private DuelManager duelManager;
    private Camera cam;
    private PlayerInputActions playerInputActions;
    private HandCardUI previousSelection;

    private void Awake() {
        cam = Camera.main;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Enable();
        playerInputActions.Player.Select.performed += SelectCard;
        previousSelection = null;
    }

    private void Start() {
        duelManager = FindFirstObjectByType<DuelManager>();
        if (duelManager == null)
            throw new Exception("Could not find DuelManager object");
        DuelStateManager stateManager = FindFirstObjectByType<DuelStateManager>();
        if (stateManager == null)
            throw new Exception("Could not find DuelStateManager object");

        duelManager.OnManaCountChanged += SetManaCountUI;
        duelManager.OnDrawCard += DrawCardUI;
        stateManager.DrawPhase.OnDrawPhase += (sender, e) => {
            for (int i = 0; i < playerUI.CardsInHand.Count; i++)
                playerUI.CardsInHand[i].SetBorderVisibility(false);
        };
        stateManager.StartPhase.OnStartPhase += SetSelectableCardsUI;
    }

    private void Update() {
        HandCardUI handCardUI = HoverDetection();
        if(handCardUI == null && previousSelection != null) {
            ExitHoverHand();
            previousSelection = null;
        }
        else if(handCardUI != null && playerUI.ContainsCard(handCardUI)) {
            if (previousSelection == null) {
                HoverHand(handCardUI);
                previousSelection = handCardUI;
            }
            else if(handCardUI != previousSelection) {
                ExitHoverCard(previousSelection);
                HoverCard(handCardUI);
                previousSelection = handCardUI;
            }
        }
    }

    private void DrawCardUI(object sender, DrawCardEventArgs args) {
        if (args.Player.Uuid == playerUI.PlayerUuid) {
            playerUI.DrawCard();
        }
    }

    private void SetManaCountUI(object sender, ManaChangedEventArgs args) {
        if (args.Player.Uuid == playerUI.PlayerUuid) {
            playerUI.SetManaCount(args.CurrentMana);
        }
    }

    private void SetSelectableCardsUI(object sender, EventArgs args) {
        playerUI.SetSelectableCards();
    }

    private void SelectCard(InputAction.CallbackContext context) {
        if (!context.performed)
            return;
        if (playerUI.PlayerUuid != duelManager.GetCurrentPlayerTurn().Uuid)
            return;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        HandCardUI cardUI = null;
        foreach(RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>()) {
                cardUI = hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
                break;
            }
        }
        if (cardUI == null)
            return;
        if (!playerUI.ContainsCard(cardUI))
            return;
        int cardIndex = playerUI.IndexOf(cardUI);
        if (cardIndex == -1)
            return;
        MatchPlayer player = duelManager.GetCurrentPlayerTurn();
        if (!player.Hand[cardIndex].IsPlayable(duelManager, player))
            return;

        Debug.Log("PlayerUIController before PlayCardInHand");
        duelManager.PlayCardInHand(player, cardIndex);
        cardUI.Select();
    }

    private HandCardUI HoverDetection() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        if(hits.Length > 0)
            Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider.GetComponent<HandCardCollisionPointer>())
                return hit.collider.GetComponent<HandCardCollisionPointer>().HandCardUI;
        }

        return null;
    }

    private void HoverHand(HandCardUI card) {
        playerUI.InspectHand();
        HoverCard(card);
    }

    private void ExitHoverHand() {
        playerUI.DefaultCardPositions();
    }

    private void HoverCard(HandCardUI card) {
        playerUI.HoverCard(card);
    }

    private void ExitHoverCard(HandCardUI card) {
        playerUI.ExitHoverCard(card);
    }
}
