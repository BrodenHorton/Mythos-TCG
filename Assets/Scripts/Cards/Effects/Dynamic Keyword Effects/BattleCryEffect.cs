public abstract class BattleCryEffect : DynamicCreatureCardEffect {

    protected abstract void BattleCryEffectHandler(object sender, ulong currentPlayerTurnId);

    public sealed override EffectKeyword GetDynamicKeyword() {
        return ServiceLocator.Get<DynamicKeywordRegistry>().Get("battle_cry");
    }
}

public abstract class DeathCryEffect : DynamicCreatureCardEffect {

    protected abstract void DeathCryEffectHandler(object sender, PlayerCardEventArgs<CreatureCard> args);

    public sealed override EffectKeyword GetDynamicKeyword() {
        return ServiceLocator.Get<DynamicKeywordRegistry>().Get("death_cry");
    }
}