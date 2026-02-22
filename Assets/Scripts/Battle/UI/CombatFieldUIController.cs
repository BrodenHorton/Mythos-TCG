using UnityEngine;

public class CombatFieldUIController : MonoBehaviour {
    [SerializeField] private CombatFieldUI combatFieldUI;

    private MatchPlayer player;

    private void Start() {
        EventBus.OnDeclareAttacker += AddAttacker;
        EventBus.OnDeclareDefender += AddDefender;
    }

    public void Init(MatchPlayer player) {
        this.player = player;
    }

    public void AddAttacker(object sender, DeclareAttackerEventArgs args) {
        if (player.Uuid != args.Defender.Uuid)
            return;

        combatFieldUI.AddAttacker(args.Card);
    }

    // TODO: Implement so the defender corresponds to a given attacker
    public void AddDefender(object sender, DeclareDefenderEventArgs args) {
        if (player.Uuid != args.Defender.Uuid)
            return;

        combatFieldUI.AddDefender(args.Card);
    }

}