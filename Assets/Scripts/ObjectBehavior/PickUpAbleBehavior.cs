using System;
using System.Collections.Generic;
using CharacterBehavior;
using GameManagement;
using UnityEngine;

namespace ObjectBehavior {
    public class PickUpAbleBehavior : MonoBehaviour {
    
        public Dictionary<AnimalTypes, bool> HasFollowerDictionary { get; private set; }
        public GameObject Barn { get; private set; }
        public bool IsBeingCarried { get; set; }

        private ScoreboardBehavior _scoreboardBehavior;

        private void Start() {
            HasFollowerDictionary = new Dictionary<AnimalTypes, bool>();
            foreach (AnimalTypes animal in Enum.GetValues(typeof(AnimalTypes))) {
                HasFollowerDictionary[animal] = false;
            }

            Barn = null;
            IsBeingCarried = false;
            _scoreboardBehavior = GameObject.Find("Scoreboard").GetComponent<ScoreboardBehavior>();
        }
        private void OnTriggerEnter(Collider other) {
            if (other.name.Contains("Goal")) {
                _scoreboardBehavior.AddPoint((AnimalTypes) Enum.Parse(typeof(AnimalTypes), other.tag));
                Barn = other.transform.parent.gameObject;
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.name.Contains("Goal")) {
                _scoreboardBehavior.RemovePoint((AnimalTypes) Enum.Parse(typeof(AnimalTypes), other.tag));
                Barn = null;
            }
        }

        public static void DeParent(GameObject gObject, AnimalTypes animalType) {
            foreach (Transform t in gObject.transform) {
                if (t.tag.Equals("PickUpAble")) {
                    t.parent = null;
                    var tempCurrentPickUpAbleRb = t.GetComponent<Rigidbody>();
                    tempCurrentPickUpAbleRb.useGravity = true;
                    tempCurrentPickUpAbleRb.isKinematic = false;
                    t.GetComponent<PickUpAbleBehavior>().HasFollowerDictionary[animalType] = false;
                }
            }
        }
    }
}
