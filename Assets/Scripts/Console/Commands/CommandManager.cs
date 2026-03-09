using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour {
    private Dictionary<string, Command> commandsByName;

    private void Awake() {
        commandsByName = new Dictionary<string, Command>();
        CreateLobbyCommand createLobbyCommand = new CreateLobbyCommand(FindFirstObjectByType<TcgLobby>());
        AddCommand(createLobbyCommand);
        ListLobbiesCommand listLobbiesCommand = new ListLobbiesCommand(FindFirstObjectByType<TcgLobby>());
        AddCommand(listLobbiesCommand);

        FindFirstObjectByType<ConsoleInputFieldUI>().OnConsoleCommandSubmission += ExecuteCommand;
    }

    private void AddCommand(Command command) {
        commandsByName.Add(command.Name.ToLower(), command);
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