using UnityEngine;

public class OpponentUIController : DuelistUIController {
    [SerializeField] private OpponentUI opponentUI;

    public override void Init(MatchPlayer player) {
        this.player = player;
        opponentUI.Init(player);
    }

    public override void SetLifePoints(int lifePoints) {
        opponentUI.SetLifePoints(lifePoints);
    }

    public override void SetManaCount(int manaCount) {
        opponentUI.SetManaCount(manaCount);
    }

    public override void DrawCard(Card card) {
        opponentUI.DrawCard(card);
    }

    public override void RemoveCardFromHand(int handIndex) {
        opponentUI.RemoveCardFromHand(handIndex);
    }

    public override DuelistUI GetDuelistUI() {
        return opponentUI;
    }
}
