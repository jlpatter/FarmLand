using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartMenu {
    public class StartMenuButtons : MonoBehaviour {
        public TMP_Dropdown tmpDropdown;
        
        public void PlayButtonPush() {
            StartMenuValue.animal = tmpDropdown.value;
            SceneManager.LoadScene(1);
        }
        
        public void ExitButtonPush() {
            Application.Quit();
        }
    }
}
