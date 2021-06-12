using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartMenu {
    public class StartMenuButtons : MonoBehaviour
    {
        public void PlayButtonPush() {
            SceneManager.LoadScene(1);
        }
        
        public void ExitButtonPush() {
            Application.Quit();
        }
    }
}
