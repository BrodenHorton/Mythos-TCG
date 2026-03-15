using Unity.Netcode;
using UnityEngine;

public class TcgMultiplayerManager : MonoBehaviour {
    public static TcgMultiplayerManager Instance {  get; private set; }

    private void Awake() {
        if(Instance != null) {
            Debug.Log("TcgMultiplayerManager already exists in scene. Destroying redundant object");
            Destroy(gameObject);
        }
        else {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartHost() {
        Debug.Log("Starting as Host");
        NetworkManager.Singleton.StartHost();
        SceneLoader.NetworkLoadScene(SceneLoader.Scene.TwoPlayerDuel);
    }

    public void StartClient() {
        Debug.Log("Starting as Client");
        NetworkManager.Singleton.StartClient();
    }
}
