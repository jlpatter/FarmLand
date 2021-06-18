using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Credits {
    public class CreditsButton : MonoBehaviour {
        public GameObject backButton;

        private void Start() {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(backButton);
        }

        public void BackButtonPush() {
            SceneManager.LoadScene(0);
        }
    }
}
