using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TCGLobby : MonoBehaviour {
    
    private async void Start() {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () => {
            Debug.Log("Signed in: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }


    public async void CreateLobby() {
        try {
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync("My First Lobby", 4);
            Debug.Log("Joined Lobby: " + lobby.Name + " Max players: " + lobby.MaxPlayers);
        }
        catch(LobbyServiceException e) {
            Debug.Log(e.Message);
        }
    }
}
