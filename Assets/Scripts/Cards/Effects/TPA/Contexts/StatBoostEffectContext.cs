
public class StatBoostEffectContext : EffectContext<CreatureCard> {
    private int atkBoost;
    private int healthBoost;
    private bool isResetAfterTurn;
    private int effectProkCount;
    private DuelStateManager stateManager;

    public StatBoostEffectContext(string id, string effectName, int atkBoost, int healthBoost, bool isResetAfterTurn) : base(id, effectName) {
        this.atkBoost = atkBoost;
        this.healthBoost = healthBoost;
        this.isResetAfterTurn = isResetAfterTurn;
        effectProkCount = 0;
    }

    public override void Init(CreatureCard card) {
        stateManager = ServiceLocator.Get<DuelStateManager>();

        EventBus.Instance.OnCalculateCreatureAttack += AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth += AddHealth;
        stateManager.EndPhase.OnEndPhasEnteredFinished += ClearEffectProks;
    }

    public override void RemoveListeners() {
        EventBus.Instance.OnCalculateCreatureAttack -= AddAttack;
        EventBus.Instance.OnCalculateCreatureHealth -= AddHealth;
        stateManager.EndPhase.OnEndPhasEnteredFinished -= ClearEffectProks;
    }

    private void AddAttack(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;
        if (effectProkCount <= 0 || atkBoost <= 0)
            return;

        args.Value += effectProkCount * atkBoost;
    }

    private void AddHealth(object sender, PlayerCardStatEventArgs<CreatureCard> args) {
        if (args.Card.Uuid != card.Uuid)
            return;
        if (effectProkCount <= 0 || healthBoost <= 0)
            return;

        args.Value += effectProkCount * healthBoost;
    }

    private void ClearEffectProks(object sender, PlayerEventArgs args) {
        if (card.PlayerId != args.PlayerId)
            return;
        if (!isResetAfterTurn)
            return;

        effectProkCount = 0;
        EventBus.Instance.InvokeOnCreatureCardEffectClientpdate(new CreatureCardPayload(card));
    }

    public int EffectProkCount { get { return effectProkCount; } set { effectProkCount = value; } }
}
