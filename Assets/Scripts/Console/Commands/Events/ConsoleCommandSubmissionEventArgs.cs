using System;

public class ConsoleCommandSubmissionEventArgs : EventArgs {
    private string cmd;
    private string[] args;

    public ConsoleCommandSubmissionEventArgs(string cmd, string[] args) {
        this.cmd = cmd;
        this.args = args;
    }

    public string Cmd { get { return cmd; } }

    public string[] Args { get { return args; } }
}
