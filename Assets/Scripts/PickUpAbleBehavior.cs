using System;
using UnityEngine;

public class PickUpAbleBehavior : MonoBehaviour {

    private ScoreboardBehavior _scoreboardBehavior;

    private void Start() {
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
