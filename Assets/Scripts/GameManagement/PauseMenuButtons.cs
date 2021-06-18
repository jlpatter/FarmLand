using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GameManagement {
    public class PauseMenuButtons : MonoBehaviour {

        public GameObject mainMenuButton;

        private void OnEnable() {
            var coroutine = SelectButtonLater();
            StartCoroutine(coroutine);
        }
        
        private IEnumerator SelectButtonLater()
        {
            yield return null;
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
