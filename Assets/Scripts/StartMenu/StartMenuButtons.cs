using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace StartMenu {
    public class StartMenuButtons : MonoBehaviour {
        public TMP_Dropdown tmpDropdown;
        public AudioSource musicSource;
        public GameObject playButton;
        public AudioMixer audioMixer;

        private void Start() {
            if (!StartMenuValue.isPlayingMusic) {
                musicSource.Play();
                StartMenuValue.isPlayingMusic = true;
            }
            
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(playButton);
        }

        public void SinglePlayerButtonPush() {
            StartMenuValue.animalP1 = tmpDropdown.value;
            StartMenuValue.isMultiplayer = false;
            SceneManager.LoadScene(1);
        }

        public void MultiPlayerButtonPush() {
            // Set main canvas to inactive
            gameObject.SetActive(false);
        }

        public void ExitButtonPush() {
            Application.Quit();
        }

        public void CreditsButtonPush() {
            SceneManager.LoadScene(2);
        }

        public void SetVolume(float volume) {
            audioMixer.SetFloat("volume", volume);
        }
    }
}
