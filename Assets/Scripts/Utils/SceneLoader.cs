using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader {
    public enum Scene {
        MainMenu,
        TwoPlayerDuel
    }

    public static void LoadScene(Scene scene) {
        SceneManager.LoadScene(scene.ToString());
    }

    public static void NetworkLoadScene(Scene scene) {
        NetworkManager.Singleton.SceneManager.LoadScene(scene.ToString(), LoadSceneMode.Single);
    }
}