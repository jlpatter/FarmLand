using System.Linq;
using UnityEngine;

namespace AnimalBehavior {
    public class CowBehavior : AIBehavior {
        private new void OnTriggerEnter(Collider other) {
            var aiBehavior = other.gameObject.GetComponent<AIBehavior>();
            if (aiBehavior != null && aiBehavior.Animal != Animal) {
                // TODO: Make it take damage instead of destroying it!
                Destroy(other.gameObject);
                var toRemoveList = aiBehavior.GameManagerBehavior.AllAnimals.Where(myTuple => myTuple.Item1 == other.gameObject).ToList();
                foreach (var removeMe in toRemoveList) {
                    aiBehavior.GameManagerBehavior.AllAnimals.Remove(removeMe);
                }
            }

            var playerBehavior = other.gameObject.GetComponent<PlayerBehavior>();
            if (playerBehavior != null && playerBehavior.Animal != Animal) {
                // TODO: Make it take damage instead of destroying it!
                Destroy(other.gameObject);
                var toRemoveList = playerBehavior.GameManagerBehavior.AllAnimals.Where(myTuple => myTuple.Item1 == other.gameObject).ToList();
                foreach (var removeMe in toRemoveList) {
                    playerBehavior.GameManagerBehavior.AllAnimals.Remove(removeMe);
                }
            }
            base.OnTriggerEnter(other);
        }
    }
}
