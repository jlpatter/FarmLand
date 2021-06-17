using UnityEngine;

namespace StartMenu {
    public class DontDestroyMusic : MonoBehaviour
    {
        private void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}
