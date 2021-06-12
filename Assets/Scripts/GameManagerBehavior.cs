using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour {
    public List<Tuple<GameObject, Animal>> AllAnimals { get; private set; }

    private void Start() {
        AllAnimals = new List<Tuple<GameObject, Animal>>();
    }
}
