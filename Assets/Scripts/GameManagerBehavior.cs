using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour {
    public List<Tuple<GameObject, Animal>> AllAnimals { get; private set; }
    public Dictionary<Animal, AnimalAttributes> AnimalAttributesDict { get; set; }

    private void Start() {
        AllAnimals = new List<Tuple<GameObject, Animal>>();
        AnimalAttributesDict = new Dictionary<Animal, AnimalAttributes>();
        AnimalAttributesDict[Animal.Rabbit] = new AnimalAttributes(5.0f, 50.0f);
        AnimalAttributesDict[Animal.Cow] = new AnimalAttributes(2.0f, 100.0f);
        AnimalAttributesDict[Animal.Pig] = new AnimalAttributes(3.0f, 75.0f);
    }
}
