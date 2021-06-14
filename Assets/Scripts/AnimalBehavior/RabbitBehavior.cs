using UnityEngine;

namespace AnimalBehavior {
    public class RabbitBehavior : AIBehavior {

        public GameObject weapon;
        
        protected override void FollowEnemy() {
            if (currentEnemy != null) {
                currentDirection = (currentEnemy.transform.position - transform.position).normalized;
                if ((currentEnemy.transform.position - transform.position).magnitude > EnemySensoryRange) {
                    currentState = AIState.IsGrazing;
                }
                else if ((currentEnemy.transform.position - transform.position).magnitude < 1.5f) {
                    currentDirection = Vector3.zero;
                    weapon.SetActive(true);
                }
            }
            else {
                currentState = AIState.IsGrazing;
            }
        }
    }
}
