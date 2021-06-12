using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace StartMenu {
    public class StartMenuButtons : MonoBehaviour {
        public Dropdown dropdown;
        
        public void PlayButtonPush() {
            StartMenuValue.animal = dropdown.value;
            SceneManager.LoadScene(1);
        }
        
        public void ExitButtonPush() {
            Application.Quit();
        }
    }
}
