using Unity.Netcode;
using System;

public class OutOfCombatState : NetworkBehaviour, CombatState {
    public event EventHandler OnOutOfCombatEntered;

    public void EnterState() {
        OnOutOfCombatEntered?.Invoke(this, EventArgs.Empty);
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
