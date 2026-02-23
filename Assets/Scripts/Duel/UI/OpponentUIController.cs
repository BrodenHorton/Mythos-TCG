using UnityEngine;

public class OpponentUIController : DuelistUIController {
    [SerializeField] private OpponentUI opponentUI;

    private void Awake() {
        EventBus.OnManaCountChanged += SetManaCount;
        EventBus.OnCreatureCardDrawn += DrawCreatureCard;
        EventBus.OnSpellCardDrawn += DrawSpellCard;
    }

    private void DrawCreatureCard(object sender, PlayerCreatureCardEventArgs args) {
        if (player.Uuid != args.Player.Uuid)
            return;

        opponentUI.DrawCreatureCard(args.Card);
    }

    private void DrawSpellCard(object sender, PlayerSpellCardEventArgs args) {
        if (player.Uuid != args.Player.Uuid)
            return;

        opponentUI.DrawSpellCard(args.Card);
    }

    public void SetManaCount(object sender, ManaChangedEventArgs args) {
        if (player.Uuid != args.Player.Uuid)
            return;

        opponentUI.SetManaCount(args.CurrentMana);
    }

    public override DuelistUI GetDuelistUI() {
        return opponentUI;
    }
}
