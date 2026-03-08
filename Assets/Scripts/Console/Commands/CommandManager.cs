using System.Collections.Generic;
using UnityEngine;

public class CommandManager : MonoBehaviour {
    [SerializeField] private List<Command> commands;

    private Dictionary<string, Command> commandsByName;

    private void Awake() {
        commandsByName = new Dictionary<string, Command>();
        foreach(Command cmd in commands)
            commandsByName.Add(cmd.name, cmd);
    }

    public void ExecuteCommand(string cmd, string[] args) {
        Command command = commandsByName[cmd];
        if(command != null)
            command.Execute(args);
    }
}