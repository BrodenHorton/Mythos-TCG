using System;
using UnityEngine;

public class PlayingFieldPlayableArea : MonoBehaviour {
    [SerializeField] private PlayerPlayingFieldUI playingFieldUI;
    [SerializeField] private Collider playableAreaCollider;
    [SerializeField] private GameObject playableAreaVisual;

    private Camera cam;

    private void Start() {
        cam = Camera.main;
        playableAreaVisual.SetActive(false);

        EventBus.OnStartHandCardDrag += ShowPlayableAreaVisual;
        EventBus.OnReleaseHandCardDrag += PlayCardOnReleaseDrag;
    }

    private void ShowPlayableAreaVisual(object sender, HandCardDragEventArgs args) {
        if (playingFieldUI.PlayerId != args.PlayerUI.PlayerId)
            return;

        playableAreaVisual.SetActive(true);
    }

    private void PlayCardOnReleaseDrag(object sender, HandCardDragEventArgs args) {
        if (playingFieldUI.PlayerId != args.PlayerUI.PlayerId)
            return;

        TcgLogger.Log("Released hand card");
        playableAreaVisual.SetActive(false);
        if (IsHoveringPlayableArea()) {
            TcgLogger.Log("Released hand card over playable area");
            EventBus.InvokeOnHandCardEnteringPlayingField(this, new HandCardEnteringPlayingFieldEventArgs(playingFieldUI, args.CardUI, args.CardIndex));
        }
    }

    private bool IsHoveringPlayableArea() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray);
        Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (RaycastHit hit in hits) {
            if (hit.collider == playableAreaCollider)
                return true;
        }

        return false;
    }
}