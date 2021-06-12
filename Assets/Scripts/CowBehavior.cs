using System.Linq;
using UnityEngine;

public class CowBehavior : AIBehavior {
    private new void Start() {
        speed = 2.0f;
        base.Start();
    }

    private new void OnTriggerEnter(Collider other) {
        var aiBehavior = other.gameObject.GetComponent<AIBehavior>();
        if (aiBehavior != null && aiBehavior.Animal != Animal) {
            // TODO: Make it take damage instead of destroying it!
            Destroy(other.gameObject);
            var toRemove = aiBehavior.GameManagerBehavior.AllAnimals.Where(myTuple => myTuple.Item1 == other.gameObject).ToList();
            foreach (var removeMe in toRemove) {
                aiBehavior.GameManagerBehavior.AllAnimals.Remove(removeMe);
            }
        }
        base.OnTriggerEnter(other);
    }
}
