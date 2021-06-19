using System.Linq;
using CharacterBehavior.PlayerBehavior;
using ObjectBehavior;
using UnityEngine;

namespace CharacterBehavior.AnimalBehavior {
    public class CowBehavior : AIBehavior {

        private const float TrampleStrength = 1.0f;

        protected override void OnTriggerEnter(Collider other) {
            TrampleAIAndPlayer(other);
            base.OnTriggerEnter(other);
        }

        private void OnTriggerStay(Collider other) {
            TrampleAIAndPlayer(other);
        }
        
        protected override void FollowEnemy() {
            if (currentEnemy != null) {
                currentDirection = (currentEnemy.transform.position - transform.position).normalized;
                if ((currentEnemy.transform.position - transform.position).magnitude > EnemySensoryRange) {
                    currentState = AIState.IsGrazing;
                }
            }
            else {
                currentState = AIState.IsGrazing;
            }
        }

        private void TrampleAIAndPlayer(Collider other) {
            var aiBehavior = other.gameObject.GetComponent<AIBehavior>();
            if (aiBehavior != null && aiBehavior.AnimalType != AnimalType) {
                aiBehavior.Health -= TrampleStrength;
                aiBehavior.HealthBar.SetHealth(aiBehavior.Health);

                if (aiBehavior.Health <= 0.0f) {
                    var toRemoveList = aiBehavior.GameManagerBehavior.AllAnimals.Where(myTuple => myTuple.Item1 == other.gameObject).ToList();
                    foreach (var removeMe in toRemoveList) {
                        aiBehavior.GameManagerBehavior.AllAnimals.Remove(removeMe);
                    }
                    
                    PickUpAbleBehavior.DeParent(other.gameObject, aiBehavior.AnimalType);
                    Destroy(other.gameObject);
                }
            }

            var playerBehavior = other.gameObject.GetComponent<PlayerBehavior.PlayerBehavior>();
            if (playerBehavior != null && playerBehavior.AnimalType != AnimalType) {
                playerBehavior.Health -= TrampleStrength;
                GameObject.Find("HealthBar").GetComponent<PlayerHealthBar>().SetHealth(playerBehavior.Health);

                if (playerBehavior.Health <= 0.0f) {
                    var toRemoveList = playerBehavior.GameManagerBehavior.AllAnimals.Where(myTuple => myTuple.Item1 == other.gameObject).ToList();
                    foreach (var removeMe in toRemoveList) {
                        playerBehavior.GameManagerBehavior.AllAnimals.Remove(removeMe);
                    }
                    
                    PickUpAbleBehavior.DeParent(other.gameObject, playerBehavior.AnimalType);
                    Destroy(other.gameObject);
                }
            }
        }
    }
}
