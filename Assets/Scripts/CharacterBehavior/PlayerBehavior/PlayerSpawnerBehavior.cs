using Cinemachine;
using StartMenu;
using UnityEngine;

namespace CharacterBehavior.PlayerBehavior {
    public class PlayerSpawnerBehavior : MonoBehaviour {

        public GameObject playerPrefab;
        public CinemachineFreeLook cinemachineFreeLook;
        public CinemachineFreeLook p2CinemachineFreeLook;
        public CinemachineInputProvider p1InputProvider;
        public CinemachineInputProvider p2InputProvider;

        private bool _isFirstSpawn;

        private void Start() {
            _isFirstSpawn = true;
        }

        private void Update() {
            if (!PlayerValue.p1IsAlive) {
                var player = Instantiate(playerPrefab, transform.position, Quaternion.identity, transform);
                cinemachineFreeLook.Follow = player.transform;
                cinemachineFreeLook.LookAt = player.transform;
                var playerBehavior = player.GetComponent<PlayerBehavior>();
                playerBehavior.playerNumber = PlayerNumber.Player1;
                playerBehavior.SetCamera("P1 Cam");
                playerBehavior.SetCinemachineFreeLook("P1 TPC");
                playerBehavior.SetHealthBar("HealthBarP1");
                playerBehavior.SetAnimal(StartMenuValue.animalP1);

                PlayerValue.p1IsAlive = true;
            }

            
            if (StartMenuValue.isMultiplayer) {
                if (!PlayerValue.p2IsAlive) {
                    var player2 = Instantiate(playerPrefab, transform.position, Quaternion.identity, transform);
                    p2CinemachineFreeLook.Follow = player2.transform;
                    p2CinemachineFreeLook.LookAt = player2.transform;
                    var player2Behavior = player2.GetComponent<PlayerBehavior>();
                    player2Behavior.playerNumber = PlayerNumber.Player2;
                    player2Behavior.SetCamera("P2 Cam");
                    player2Behavior.SetCinemachineFreeLook("P2 TPC");
                    player2Behavior.SetHealthBar("HealthBarP2");
                    player2Behavior.SetAnimal(StartMenuValue.animalP2);
            
                    p1InputProvider.PlayerIndex = 0;
                    p2InputProvider.PlayerIndex = 1;
                    PlayerValue.p2IsAlive = true;
                }
            }
            else {
                if (_isFirstSpawn) {
                    Destroy(GameObject.Find("P2 TPC"));
                    Destroy(GameObject.Find("P2 Cam"));
                    Destroy(GameObject.Find("HealthBarP2"));

                    GameObject.Find("P1 Cam").GetComponent<Camera>().rect = new Rect(0, 0, 1, 1);
                    _isFirstSpawn = false;
                }
            }
        }
    }
}
