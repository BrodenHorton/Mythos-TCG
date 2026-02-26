using UnityEngine;

public class OpponentUIController : DuelistUIController {
    [SerializeField] private OpponentUI opponentUI;

    private void Start() {
        EventBus.OnLifePointsChanged += SetLifePoints;
        EventBus.OnManaCountChanged += SetManaCount;
        EventBus.OnCreatureCardDrawn += DrawCreatureCard;
        EventBus.OnSpellCardDrawn += DrawSpellCard;
    }

    public override void Init(MatchPlayer player) {
        this.player = player;
        opponentUI.Init(player);
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

    private void SetLifePoints(object sender, LifePointsChangedEventArgs args) {
        if (args.Player.Uuid == player.Uuid)
            opponentUI.SetLifePoints(args.LifePoints);
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
