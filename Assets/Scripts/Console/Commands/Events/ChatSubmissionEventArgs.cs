using System;

public class ChatSubmissionEventArgs : EventArgs {
    private string message;

    public ChatSubmissionEventArgs(string message) {
        this.message = message;
    }

    public string Message { get { return message; } }
}
