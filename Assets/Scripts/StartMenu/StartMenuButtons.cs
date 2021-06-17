using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StartMenu {
    public class StartMenuButtons : MonoBehaviour {
        public TMP_Dropdown tmpDropdown;
        public AudioSource musicSource;

        private void Start() {
            if (!StartMenuValue.isPlayingMusic) {
                musicSource.Play();
                StartMenuValue.isPlayingMusic = true;
            }
        }

        public void PlayButtonPush() {
            StartMenuValue.animal = tmpDropdown.value;
            SceneManager.LoadScene(1);
        }

        public void ExitButtonPush() {
            Application.Quit();
        }

        public void CreditsButtonPush() {
            SceneManager.LoadScene(2);
        }
    }
}
