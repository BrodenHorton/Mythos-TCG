using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class TcgRelay : MonoBehaviour, TcgLogSender {
    public static TcgRelay Instance { get; private set; }

    private void Awake() {
        if (Instance != null) {
            Debug.LogWarning("TcgRelay already exists in scene. Destroying redundant object.");
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public async Task<string> CreateRelay() {
        string joinCode = "";
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            TcgLogger.Log(this, "Join Code: " + joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(allocation.ToRelayServerData("dtls"));
            TcgMultiplayerManager.Instance.StartHost();
        }
        catch(RelayServiceException e) {
            Debug.LogError(e.Message);
        }

        return joinCode;
    }

    public async void JoinRelay(string joinCode) {
        try {
            TcgLogger.Log(this, "Joining relay with code: " + joinCode);
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(joinAllocation.ToRelayServerData("dtls"));
            TcgMultiplayerManager.Instance.StartClient();
        }
        catch(RelayServiceException e) {
            Debug.LogError(e.Message);
        }
    }

    public string GetLogPrefix() {
        return "[&bRelay&f]";
    }
}
