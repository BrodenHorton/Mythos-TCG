using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour {
    private Dictionary<string, Command> commandsByName;

    private void Awake() {
        commandsByName = new Dictionary<string, Command>();
        CreateLobbyCommand createLobbyCommand = new CreateLobbyCommand(FindFirstObjectByType<TCGLobby>());
        commandsByName.Add(createLobbyCommand.Name.ToLower(), createLobbyCommand);

        FindFirstObjectByType<ConsoleInputFieldUI>().OnConsoleCommandSubmission += ExecuteCommand;
    }

    private void ExecuteCommand(object sender, ConsoleCommandSubmissionEventArgs args) {
        ExecuteCommand(args.Cmd, args.Args);
    }

    public void ExecuteCommand(string cmd, string[] args) {
        if(!commandsByName.ContainsKey(cmd.ToLower())) {
            Debug.Log("Invalid command " + cmd.ToLower());
            return;
        }

        Command command = commandsByName[cmd.ToLower()];
        command.Execute(args);
    }
}