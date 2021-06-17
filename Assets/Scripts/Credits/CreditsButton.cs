using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsButton : MonoBehaviour {
    public void BackButtonPush() {
        SceneManager.LoadScene(0);
    }
}
