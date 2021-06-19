using System;
using Cinemachine;
using StartMenu;
using UnityEngine;

namespace CharacterBehavior.PlayerBehavior {
    public class PlayerSpawnerBehavior : MonoBehaviour {

        public GameObject playerPrefab;
        public CinemachineFreeLook cinemachineFreeLook;

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
            }
        }
    }
}
