using System;
using System.Collections.Generic;
using System.Linq;
using GameManagement;
using ObjectBehavior;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CharacterBehavior.AnimalBehavior {
    public class AIBehavior : MonoBehaviour {
        public AnimalTypes AnimalType { get; private set; }
        public GameManagerBehavior GameManagerBehavior { get; private set; }
        public AIHealthBar HealthBar { get; private set; }
        public float Health { get; set; }
    
        public Transform groundCheckTransform;
        public LayerMask groundMask;

        protected AIState currentState;
        private CharacterController _characterController;
        private float _turnSmoothVelocity;
        private Vector3 _velocity;
        private bool _isGrounded;
        private float _timer;
        protected Vector3 currentDirection;
        protected GameObject currentEnemy;
        private GameObject _targetPickUpAble;
        private PickUpAbleBehavior _targetPickUpAbleBehavior;
        private GameObject _myBarn;
        private GameObject _myBarnEntrance;
        private GameObject _myBarnInterior;
        private List<Tuple<GameObject, PickUpAbleBehavior>> _pickUpAbleList;
        private bool _isCarryingPickUpAble;
        private bool _isInBarn;
        private float _speed;

        private const float TurnSmoothTime = 0.1f;
        private const float Gravity = -9.81f;
        private const float GroundDistance = 0.4f;
        protected const float EnemySensoryRange = 20.0f;

        protected enum AIState {
            IsGrazing,
            FollowEnemy,
            IsTravelingToPickUpAble,
            IsTravelingToEnemyBarnEntrance,
            IsTravelingToEnemyBarnInterior,
            IsTravelingToBarnEntrance,
            IsTravelingToBarnInterior
        }

        private void Awake() {
            if (name.Contains("Rabbit")) {
                AnimalType = AnimalTypes.Rabbit;
            }
            else if (name.Contains("Cow")) {
                AnimalType = AnimalTypes.Cow;
            }
            else if (name.Contains("Pig")) {
                AnimalType = AnimalTypes.Pig;
            }
            else if (name.Contains("Chicken")) {
                AnimalType = AnimalTypes.Chicken;
            }
        }

        protected void Start() {
            _characterController = GetComponent<CharacterController>();
            _timer = -1.0f;
            _targetPickUpAble = null;
            _targetPickUpAbleBehavior = null;
            _isCarryingPickUpAble = false;
            _isInBarn = false;
            currentState = AIState.IsGrazing;
            currentEnemy = null;

            GameManagerBehavior = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
            GameManagerBehavior.AllAnimals.Add(new Tuple<GameObject, AnimalTypes>(gameObject, AnimalType));
            HealthBar = gameObject.GetComponentInChildren<AIHealthBar>();
            _speed = GameManagerBehavior.AnimalAttributesDict[AnimalType].Speed;
            Health = GameManagerBehavior.AnimalAttributesDict[AnimalType].Health;
            HealthBar.SetMaxHealth(Health);
            HealthBar.SetHealth(Health);

            _myBarn = GameObject.Find(AnimalType + "Barn");
            _myBarnInterior = _myBarn.transform.Find("Middle").gameObject;
            _myBarnEntrance = _myBarn.transform.Find("Entrance").gameObject;
            
            var pickUpAbles = GameObject.FindGameObjectsWithTag("PickUpAble");
            _pickUpAbleList = new List<Tuple<GameObject, PickUpAbleBehavior>>();
            foreach (var pickUpAble in pickUpAbles) {
                _pickUpAbleList.Add(new Tuple<GameObject, PickUpAbleBehavior>(pickUpAble, pickUpAble.GetComponent<PickUpAbleBehavior>()));
            }
        }

        private void Update() {

            _isGrounded = Physics.CheckSphere(groundCheckTransform.position, GroundDistance, groundMask);

            if (_isGrounded && _velocity.y < 0) {
                _velocity.y = -2.0f;
            }

            switch (currentState) {
                case AIState.IsGrazing:
                    if (_isInBarn) {
                        currentState = AIState.IsTravelingToBarnEntrance;
                    }
                    else {
                        PickRandomDirection();
                        FindEnemy();
                        FindPickUpAble();
                    }
                    break;
                case AIState.FollowEnemy:
                    FollowEnemy();
                    break;
                case AIState.IsTravelingToPickUpAble:
                    FollowPickUpAble();
                    PickUpAndDropStuff();
                    break;
                case AIState.IsTravelingToEnemyBarnEntrance:
                    FollowEnemyBarnEntrance();
                    break;
                case AIState.IsTravelingToEnemyBarnInterior:
                    FollowEnemyBarnInterior();
                    PickUpAndDropStuff();
                    break;
                case AIState.IsTravelingToBarnEntrance:
                    FollowBarnEntrance();
                    break;
                case AIState.IsTravelingToBarnInterior:
                    FollowBarnInterior();
                    PickUpAndDropStuff();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (currentDirection.magnitude >= 0.1f) {

                var targetAngle = Mathf.Atan2(currentDirection.x, currentDirection.z) * Mathf.Rad2Deg;
                var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

                var moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                _characterController.Move(moveDirection.normalized * (_speed * Time.deltaTime));
            }

            _velocity.y += Gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        protected virtual void OnTriggerEnter(Collider other) {
            if (other.name.Equals("Weapon")) {
                RemoveHealth(GameManagerBehavior.SwordDamage);
            }

            if (other.name.Contains("Axe")) {
                RemoveHealth(GameManagerBehavior.AxeDamage);
            }

            if (other.name.Contains("Goal")) {
                _isInBarn = true;
            }
        }

        private void RemoveHealth(float weaponDamage) {
            Health -= weaponDamage;
            HealthBar.SetHealth(Health);

            if (Health <= 0.0f) {
                var toRemoveList = GameManagerBehavior.AllAnimals.Where(tuple => gameObject == tuple.Item1).ToList();
                foreach (var removeMe in toRemoveList) {
                    GameManagerBehavior.AllAnimals.Remove(removeMe);
                }

                PickUpAbleBehavior.DeParent(gameObject, AnimalType);
                Destroy(gameObject);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (other.name.Contains("Goal")) {
                _isInBarn = false;
            }
        }

        private void OnDestroy() {
            if (_targetPickUpAble != null) {
                if (_isCarryingPickUpAble) {
                    _targetPickUpAbleBehavior.IsBeingCarried = false;
                }
                _targetPickUpAbleBehavior.HasFollowerDictionary[AnimalType] = false;
                _targetPickUpAble = null;
                _targetPickUpAbleBehavior = null;
            }
        }

        private void FindPickUpAble() {
            foreach (var (pickUpAble, pickUpAbleBehavior) in _pickUpAbleList) {
                if (!pickUpAbleBehavior.HasFollowerDictionary[AnimalType] && !pickUpAbleBehavior.IsBeingCarried) {
                    if (pickUpAbleBehavior.Barn == null) {
                        SetPickupAble(pickUpAble, pickUpAbleBehavior);
                        break;
                    }
                    else {
                        if (!pickUpAbleBehavior.Barn.Equals(_myBarn)) {
                            SetPickupAble(pickUpAble, pickUpAbleBehavior);
                            break;
                        }
                    }
                }
            }
        }

        private void SetPickupAble(GameObject pickUpAble, PickUpAbleBehavior pickUpAbleBehavior) {
            _targetPickUpAble = pickUpAble;
            _targetPickUpAbleBehavior = pickUpAbleBehavior;
            if (_targetPickUpAbleBehavior.Barn != null) {
                currentState = AIState.IsTravelingToEnemyBarnEntrance;
            }
            else {
                currentState = AIState.IsTravelingToPickUpAble;
            }
            pickUpAbleBehavior.HasFollowerDictionary[AnimalType] = true;
        }

        private void FindEnemy() {
            foreach (var (animalGameObject, animal) in GameManagerBehavior.AllAnimals) {
                if ((animalGameObject.transform.position - transform.position).magnitude < EnemySensoryRange && animal != AnimalType) {
                    currentState = AIState.FollowEnemy;
                    currentEnemy = animalGameObject;
                }
            }
        }

        protected virtual void FollowEnemy() {
            if (currentEnemy != null) {
                currentDirection = (currentEnemy.transform.position - transform.position).normalized;
                if ((currentEnemy.transform.position - transform.position).magnitude > EnemySensoryRange) {
                    currentState = AIState.IsGrazing;
                }
                else if ((currentEnemy.transform.position - transform.position).magnitude < 1.5f) {
                    currentDirection = Vector3.zero;
                }
            }
            else {
                currentState = AIState.IsGrazing;
            }
        }

        private void FollowPickUpAble() {
            if (!_targetPickUpAbleBehavior.IsBeingCarried) {
                if (_targetPickUpAbleBehavior.Barn == null) {
                    currentDirection = (_targetPickUpAble.transform.position - transform.position).normalized;
                }
                else {
                    currentState = AIState.IsTravelingToEnemyBarnEntrance;
                }
            }
            else {
                currentState = AIState.IsGrazing;
            }
        }

        private void FollowEnemyBarnEntrance() {
            if (_targetPickUpAbleBehavior.Barn != null) {
                currentDirection = (_targetPickUpAbleBehavior.Barn.transform.Find("Entrance").position - transform.position).normalized;
                if ((_targetPickUpAbleBehavior.Barn.transform.Find("Entrance").position - transform.position).magnitude < 2.0f) {
                    if (_isCarryingPickUpAble) {
                        currentState = AIState.IsTravelingToBarnEntrance;
                    }
                    else {
                        currentState = AIState.IsTravelingToEnemyBarnInterior;
                    }
                }
            }
            else {
                currentState = AIState.IsTravelingToPickUpAble;
            }
        }

        private void FollowEnemyBarnInterior() {
            currentDirection = (_targetPickUpAble.transform.position - transform.position).normalized;
        }

        private void FollowBarnEntrance() {
            currentDirection = (_myBarnEntrance.transform.position - transform.position).normalized;
            if ((_myBarnEntrance.transform.position - transform.position).magnitude < 2.0f) {
                if (_isCarryingPickUpAble) {
                    currentState = AIState.IsTravelingToBarnInterior;
                }
                else {
                    currentState = AIState.IsGrazing;
                }
            }
        }

        private void FollowBarnInterior() {
            currentDirection = (_myBarnInterior.transform.position - transform.position).normalized;
            if ((_myBarnInterior.transform.position - transform.position).magnitude < 2.0f) {
                currentState = AIState.IsGrazing;
            }
        }
    
        private void PickUpAndDropStuff() {
            if (!_isCarryingPickUpAble) {
                var tempMagnitude = (_targetPickUpAble.transform.position - transform.position).magnitude;
                if (tempMagnitude < 2.0f) {
                    _targetPickUpAble.transform.position = transform.position + new Vector3(0.0f, _targetPickUpAble.transform.localScale.y, 0.0f);
                    _targetPickUpAble.transform.parent = transform;
                    var tempCurrentPickUpAbleRb = _targetPickUpAble.GetComponent<Rigidbody>();
                    tempCurrentPickUpAbleRb.useGravity = false;
                    tempCurrentPickUpAbleRb.isKinematic = true;
                    _isCarryingPickUpAble = true;
                    _targetPickUpAbleBehavior.IsBeingCarried = true;
                    foreach (AnimalTypes animal in Enum.GetValues(typeof(AnimalTypes))) {
                        if (animal != AnimalType) {
                            _targetPickUpAbleBehavior.HasFollowerDictionary[animal] = false;
                        }
                    }
                    if (_isInBarn) {
                        currentState = AIState.IsTravelingToEnemyBarnEntrance;
                    }
                    else {
                        currentState = AIState.IsTravelingToBarnEntrance;
                    }
                }
            }
            else {
                // TODO: Maybe replace this with a boolean?
                if (currentState == AIState.IsGrazing) {
                    var tempCurrentPickUpAbleRb = _targetPickUpAble.GetComponent<Rigidbody>();
                    tempCurrentPickUpAbleRb.useGravity = true;
                    tempCurrentPickUpAbleRb.isKinematic = false;
                    _targetPickUpAble.transform.localPosition += new Vector3(0.0f, 0.0f, _targetPickUpAble.transform.localScale.z);
                    _targetPickUpAble.transform.parent = null;
                    _targetPickUpAbleBehavior.HasFollowerDictionary[AnimalType] = false;
                    _targetPickUpAbleBehavior.IsBeingCarried = false;
                    _targetPickUpAbleBehavior = null;
                    _targetPickUpAble = null;
                    _isCarryingPickUpAble = false;
                }
            }
        }

        private void PickRandomDirection() {
            _timer -= Time.deltaTime;
            if (_timer < 0.0f) {
                currentDirection = new Vector3(Random.Range(-1, 2), 0.0f, Random.Range(-1, 2)).normalized;
                _timer = Random.Range(1.0f, 3.0f);
            }
        }
    }
}
