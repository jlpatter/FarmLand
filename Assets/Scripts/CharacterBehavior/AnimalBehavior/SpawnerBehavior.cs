using UnityEngine;

namespace CharacterBehavior.AnimalBehavior {
    public class SpawnerBehavior : MonoBehaviour {
        public GameObject prefabToSpawn;
        public int numToSpawn;
    
        private void Start() {
            for (var i = 0; i < numToSpawn; i++) {
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity, transform);
            }
        }

        private void Update() {
            if (transform.childCount < numToSpawn) {
                Instantiate(prefabToSpawn, transform.position, Quaternion.identity, transform);
            }
        }
    }
}
