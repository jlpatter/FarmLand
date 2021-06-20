using MLAPI;
using MLAPI.SceneManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace StartMenu {
    public class StartMenuButtons : MonoBehaviour {
        public TMP_Dropdown tmpDropdown;
        public AudioSource musicSource;
        public GameObject hostButton;
        public AudioMixer audioMixer;

        private void Start() {
            if (!StartMenuValue.isPlayingMusic) {
                musicSource.Play();
                StartMenuValue.isPlayingMusic = true;
            }
            
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(hostButton);
        }

        public void HostButtonPush() {
            StartMenuValue.animal = tmpDropdown.value;
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
            StartMenuValue.gameHasStarted = true;
        }

        public void JoinButtonPush() {
            StartMenuValue.animal = tmpDropdown.value;
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
            StartMenuValue.gameHasStarted = true;
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
