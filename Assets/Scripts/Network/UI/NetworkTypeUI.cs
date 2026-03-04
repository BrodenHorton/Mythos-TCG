using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class NetworkTypeUI : MonoBehaviour {
    [SerializeField] private Button host;
    [SerializeField] private Button client;

    private void Awake() {
        host.onClick.AddListener(() => {
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        });
        client.onClick.AddListener(() => {
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        });
    }
}
