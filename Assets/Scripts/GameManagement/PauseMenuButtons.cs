using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameManagement {
    public class PauseMenuButtons : MonoBehaviour {

        public void PushExitToMainMenuButton() {
            SceneManager.LoadScene(0);
        }
    
        public void PushExitButton() {
            Application.Quit();
        }
    }
}
