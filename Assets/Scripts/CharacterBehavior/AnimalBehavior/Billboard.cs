using UnityEngine;

namespace CharacterBehavior.AnimalBehavior {
    public class Billboard : MonoBehaviour
    {
        private void LateUpdate() {
            transform.LookAt(transform.position + Camera.main.transform.forward);
        }
    }
}
