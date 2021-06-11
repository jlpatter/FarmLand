using System;
using System.Collections.Generic;
using UnityEngine;

public class PickUpAbleBehavior : MonoBehaviour {
    
    public Dictionary<Animal, bool> HasFollowerDictionary { get; private set; }

    private ScoreboardBehavior _scoreboardBehavior;

    private void Start() {
        HasFollowerDictionary = new Dictionary<Animal, bool>();
        foreach (Animal animal in Enum.GetValues(typeof(Animal))) {
            HasFollowerDictionary[animal] = false;
        }
        _scoreboardBehavior = GameObject.Find("Scoreboard").GetComponent<ScoreboardBehavior>();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.name.Contains("Goal")) {
            _scoreboardBehavior.AddPoint((Animal) Enum.Parse(typeof(Animal), other.tag));
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.name.Contains("Goal")) {
            _scoreboardBehavior.RemovePoint((Animal) Enum.Parse(typeof(Animal), other.tag));
        }
    }
}
