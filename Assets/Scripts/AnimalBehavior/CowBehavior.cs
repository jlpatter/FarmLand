using System.Linq;
using UnityEngine;

namespace AnimalBehavior {
    public class CowBehavior : AIBehavior {

        private const float TrampleStrength = 5.0f;
        
        private new void OnTriggerEnter(Collider other) {
            var aiBehavior = other.gameObject.GetComponent<AIBehavior>();
            if (aiBehavior != null && aiBehavior.AnimalType != AnimalType) {
                aiBehavior.Health -= TrampleStrength;
                aiBehavior.HealthBar.SetHealth(aiBehavior.Health);

                if (aiBehavior.Health <= 0.0f) {
                    var toRemoveList = aiBehavior.GameManagerBehavior.AllAnimals.Where(myTuple => myTuple.Item1 == other.gameObject).ToList();
                    foreach (var removeMe in toRemoveList) {
                        aiBehavior.GameManagerBehavior.AllAnimals.Remove(removeMe);
                    }
                    Destroy(other.gameObject);
                }
            }

            var playerBehavior = other.gameObject.GetComponent<PlayerBehavior>();
            if (playerBehavior != null && playerBehavior.AnimalType != AnimalType) {
                playerBehavior.Health -= TrampleStrength;
                GameObject.Find("HealthBar").GetComponent<PlayerHealthBar>().SetHealth(playerBehavior.Health);

                if (playerBehavior.Health <= 0.0f) {
                    var toRemoveList = playerBehavior.GameManagerBehavior.AllAnimals.Where(myTuple => myTuple.Item1 == other.gameObject).ToList();
                    foreach (var removeMe in toRemoveList) {
                        playerBehavior.GameManagerBehavior.AllAnimals.Remove(removeMe);
                    }
                    Destroy(other.gameObject);
                }
            }
            base.OnTriggerEnter(other);
        }
    }
}
