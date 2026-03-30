using UnityEngine;
using UnityEngine.InputSystem;

public class GameInputManager : MonoBehaviour {
    public static GameInputManager Instance { get; private set; }

    private PlayerInputActions playerInputActions;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("Instance of GameInputManager already exisits in scene. Destroying redundant object");
            Destroy(Instance.gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        playerInputActions = new PlayerInputActions();
        SwitchCurrentActionMap(playerInputActions.Player);
    }

    public void SwitchCurrentActionMap(InputActionMap actionMap) {
        foreach (InputActionMap entry in playerInputActions.asset.actionMaps) {
            if (entry.id == actionMap.id)
                entry.Enable();
            else
                entry.Disable();
        }
    }

    public PlayerInputActions PlayerInputActions { get { return playerInputActions; } }
}