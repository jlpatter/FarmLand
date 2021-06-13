using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour {
    public List<Tuple<GameObject, AnimalTypes>> AllAnimals { get; private set; }
    public Dictionary<AnimalTypes, AnimalAttributes> AnimalAttributesDict { get; set; }

    private void Start() {
        AllAnimals = new List<Tuple<GameObject, AnimalTypes>>();
        AnimalAttributesDict = new Dictionary<AnimalTypes, AnimalAttributes>();
        AnimalAttributesDict[AnimalTypes.Rabbit] = new AnimalAttributes(5.0f, 50.0f);
        AnimalAttributesDict[AnimalTypes.Cow] = new AnimalAttributes(4.6f, 100.0f);
        AnimalAttributesDict[AnimalTypes.Pig] = new AnimalAttributes(4.8f, 75.0f);
        // TODO: Add Chicken Here!
    }
}
