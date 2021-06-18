using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GameManagement {
    public class PauseMenuButtons : MonoBehaviour {

        public GameObject mainMenuButton;

        private void Start() {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mainMenuButton);
        }

        public void PushExitToMainMenuButton() {
            SceneManager.LoadScene(0);
        }
    
        public void PushExitButton() {
            Application.Quit();
        }
    }
}
