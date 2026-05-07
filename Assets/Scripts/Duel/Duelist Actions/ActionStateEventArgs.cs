using System;

public class ActionStateEventArgs : EventArgs {
    private bool hasActionFocus;
    private string actionMessage;

    public ActionStateEventArgs(bool hasActionFocus, string actionMessage) {
        this.hasActionFocus = hasActionFocus;
        this.actionMessage = actionMessage;
    }

    public bool HasActionFocus { get { return hasActionFocus; } }

    public string ActionMessage { get { return actionMessage; } }
}