using UnityEngine;

namespace ObjectBehavior {
    public class WeaponBehavior : MonoBehaviour {
    
        // Being used by animation event
        public void FinishSwing() {
            gameObject.SetActive(false);
        }
    }
}
