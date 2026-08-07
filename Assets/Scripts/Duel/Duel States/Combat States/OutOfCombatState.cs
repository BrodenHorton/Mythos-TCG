using Unity.Netcode;
using System;

public class OutOfCombatState : NetworkBehaviour, CombatState {
    public event EventHandler OnOutOfCombatStateEntered;

    public void EnterState() {
        OnOutOfCombatStateEntered?.Invoke(this, EventArgs.Empty);
    }

    public void UpdateState() { }

    public bool CanPlaySetupCards() {
        return false;
    }

    public bool CanPlaySpellCards() {
        return false;
    }

    public bool CanDeclareAttackers() {
        return false;
    }

    public bool CanDeclareDefenders() {
        return false;
    }
}
