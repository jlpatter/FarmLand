using System;
using Cinemachine;
using StartMenu;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CharacterBehavior.PlayerBehavior {
    public class PlayerSpawnerBehavior : MonoBehaviour {

        public GameObject playerPrefab;
        public CinemachineFreeLook cinemachineFreeLook;
        public CinemachineFreeLook p2CinemachineFreeLook;
        public CinemachineInputProvider p1InputProvider;
        public CinemachineInputProvider p2InputProvider;

        private void Start() {
            switch (StartMenuValue.animal) {
                case 0:
                    transform.position = GameObject.Find(AnimalTypes.Rabbit + "Spawner").transform.position;
                    break;
                case 1:
                    transform.position = GameObject.Find(AnimalTypes.Cow + "Spawner").transform.position;
                    break;
                case 2:
                    transform.position = GameObject.Find(AnimalTypes.Pig + "Spawner").transform.position;
                    break;
                case 3:
                    transform.position = GameObject.Find(AnimalTypes.Chicken + "Spawner").transform.position;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Update() {
            if (transform.childCount < 1) {
                var player = Instantiate(playerPrefab, transform.position, Quaternion.identity, transform);
                cinemachineFreeLook.Follow = player.transform;
                cinemachineFreeLook.LookAt = player.transform;
                var playerBehavior = player.GetComponent<PlayerBehavior>();
                playerBehavior.GetCamera("P1 Cam");
                playerBehavior.GetCinemachineFreeLook("P1 TPC");
                var player2 = Instantiate(playerPrefab, transform.position, Quaternion.identity, transform);
                p2CinemachineFreeLook.Follow = player2.transform;
                p2CinemachineFreeLook.LookAt = player2.transform;
                var player2Behavior = player2.GetComponent<PlayerBehavior>();
                player2Behavior.GetCamera("P2 Cam");
                player2Behavior.GetCinemachineFreeLook("P2 TPC");
                
                p1InputProvider.PlayerIndex = 0;
                p2InputProvider.PlayerIndex = 1;
            }
        }
    }
}
