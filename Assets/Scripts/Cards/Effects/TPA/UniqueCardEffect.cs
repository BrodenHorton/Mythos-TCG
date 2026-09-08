using System;
using System.Collections.Generic;

public class UniqueCardEffect<TCard> : CreatureCardEffect where TCard : Card {
    private List<IEffectSequence<CreatureCard>> sequences;
    private string rawDescription;

    public UniqueCardEffect(string rawDescription) {
        this.rawDescription = rawDescription;
    }

    public override void Init(CreatureCard card) {
        for(int i = 0; i < sequences.Count; i++)
            sequences[i].Init(card);
    }

    public override void RemoveListeners() {
        for (int i = 0; i < sequences.Count; i++)
            sequences[i].RemoveListeners();
    }

    public void AddEffectSequence(IEffectSequence<CreatureCard> sequence) {
        sequences.Add(sequence);
    }

    public override string GetRawDescription() {
        return rawDescription;
    }

    public override CreatureCardEffectPayload GetEffectPayload() {
        throw new NotImplementedException();
    }

    public override CreatureCardEffect Clone() {
        throw new NotImplementedException();
    }
}
