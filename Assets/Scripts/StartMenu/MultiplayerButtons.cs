using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartMenu {
    public class MultiplayerButtons : MonoBehaviour {
        public TMP_Dropdown tmpDropdownPlayer1;
        public TMP_Dropdown tmpDropdownPlayer2;
        public GameObject mainCanvas;
        
        public void PlayButtonPush() {
            StartMenuValue.animalP1 = tmpDropdownPlayer1.value;
            StartMenuValue.animalP2 = tmpDropdownPlayer2.value;
            StartMenuValue.isMultiplayer = true;
            SceneManager.LoadScene(1);
        }

        public void BackButtonPush() {
            mainCanvas.SetActive(true);
        }
    }
}
