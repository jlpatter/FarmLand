using MLAPI;
using StartMenu;
using UnityEngine;

namespace CharacterBehavior.AnimalBehavior {
    public class SpawnerBehavior : NetworkBehaviour {
        public GameObject prefabToSpawn;
        public int numToSpawn;

        private void Update() {
            if (StartMenuValue.gameHasStarted && IsHost) {
                if (transform.childCount < numToSpawn) {
                    var go = Instantiate(prefabToSpawn, transform.position, Quaternion.identity, transform);
                    go.GetComponent<NetworkObject>().Spawn();
                }
            }
        }
    }
}
