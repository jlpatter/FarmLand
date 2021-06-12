using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnimalBehavior {
    public class AIBehavior : MonoBehaviour {
        public AnimalTypes AnimalType { get; private set; }
        public GameManagerBehavior GameManagerBehavior { get; private set; }
    
        public Transform groundCheckTransform;
        public LayerMask groundMask;
    
        private AIState _currentState;
        private CharacterController _characterController;
        private float _turnSmoothVelocity;
        private Vector3 _velocity;
        private bool _isGrounded;
        private float _timer;
        private Vector3 _currentDirection;
        private GameObject _currentEnemy;
        private GameObject _targetPickUpAble;
        private GameObject _barnEntrance;
        private GameObject _barnInterior;
        private bool _hasPickUpAble;
        private float _speed;
        private float _health;
    
        private const float TurnSmoothTime = 0.1f;
        private const float Gravity = -9.81f;
        private const float GroundDistance = 0.4f;
        private const float EnemySensoryRange = 20.0f;
        private const float WeaponDamage = 20.0f;

        private enum AIState {
            IsGrazing,
            FindingPickUpAble,
            FollowEnemy,
            IsTravelingToPickUpAble,
            IsTravelingToBarnEntrance,
            IsTravelingToBarnInterior
        }

        protected void Start() {
            _characterController = GetComponent<CharacterController>();
            _timer = -1.0f;
            _targetPickUpAble = null;
            _hasPickUpAble = false;
            _currentState = AIState.FindingPickUpAble;
            _currentEnemy = null;

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

            GameManagerBehavior = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
            GameManagerBehavior.AllAnimals.Add(new Tuple<GameObject, AnimalTypes>(gameObject, AnimalType));
            _speed = GameManagerBehavior.AnimalAttributesDict[AnimalType].Speed;
            _health = GameManagerBehavior.AnimalAttributesDict[AnimalType].Health;
        
            _barnInterior = GameObject.Find(AnimalType + "Barn").transform.Find("Middle").gameObject;
            _barnEntrance = GameObject.Find(AnimalType + "Barn").transform.Find("Entrance").gameObject;
        }

        private void Update() {

            _isGrounded = Physics.CheckSphere(groundCheckTransform.position, GroundDistance, groundMask);

            if (_isGrounded && _velocity.y < 0) {
                _velocity.y = -2.0f;
            }

            switch (_currentState) {
                case AIState.IsGrazing:
                    PickRandomDirection();
                    FindEnemy();
                    break;
                case AIState.FindingPickUpAble:
                    FindPickUpAble();
                    break;
                case AIState.FollowEnemy:
                    FollowEnemy();
                    break;
                case AIState.IsTravelingToPickUpAble:
                    FollowPickUpAble();
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
            if (_currentDirection.magnitude >= 0.1f) {

                var targetAngle = Mathf.Atan2(_currentDirection.x, _currentDirection.z) * Mathf.Rad2Deg;
                var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
                transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

                var moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
                _characterController.Move(moveDirection.normalized * (_speed * Time.deltaTime));
            }

            _velocity.y += Gravity * Time.deltaTime;
            _characterController.Move(_velocity * Time.deltaTime);
        }

        protected void OnTriggerEnter(Collider other) {
            if (other.name.Equals("Weapon")) {
                _health -= WeaponDamage;

                if (_health <= 0.0f) {
                    var toRemoveList = GameManagerBehavior.AllAnimals.Where(tuple => gameObject == tuple.Item1).ToList();
                    foreach (var removeMe in toRemoveList) {
                        GameManagerBehavior.AllAnimals.Remove(removeMe);
                    }
                    Destroy(gameObject);
                }
            }
        }

        private void FindPickUpAble() {
            var pickUpAbles = GameObject.FindGameObjectsWithTag("PickUpAble");
            foreach (var pickUpAble in pickUpAbles) {
                var pickUpAbleBehavior = pickUpAble.GetComponent<PickUpAbleBehavior>();
                if (!pickUpAbleBehavior.HasFollowerDictionary[AnimalType]) {
                    _targetPickUpAble = pickUpAble;
                    _currentState = AIState.IsTravelingToPickUpAble;
                    pickUpAbleBehavior.HasFollowerDictionary[AnimalType] = true;
                    break;
                }
            }

            if (_targetPickUpAble == null) {
                _currentState = AIState.IsGrazing;
            }
        }

        private void FindEnemy() {
            foreach (var (animalGameObject, animal) in GameManagerBehavior.AllAnimals) {
                if ((animalGameObject.transform.position - transform.position).magnitude < EnemySensoryRange && animal != AnimalType) {
                    _currentState = AIState.FollowEnemy;
                    _currentEnemy = animalGameObject;
                }
            }
        }

        private void FollowEnemy() {
            if (_currentEnemy != null) {
                _currentDirection = (_currentEnemy.transform.position - transform.position).normalized;
                if ((_currentEnemy.transform.position - transform.position).magnitude > EnemySensoryRange) {
                    _currentState = AIState.IsGrazing;
                }
            }
            else {
                _currentState = AIState.IsGrazing;
            }
        }

        private void FollowPickUpAble() {
            _currentDirection = (_targetPickUpAble.transform.position - transform.position).normalized;
        }

        private void FollowBarnEntrance() {
            _currentDirection = (_barnEntrance.transform.position - transform.position).normalized;
            if ((_barnEntrance.transform.position - transform.position).magnitude < 2.0f) {
                _currentState = AIState.IsTravelingToBarnInterior;
            }
        }

        private void FollowBarnInterior() {
            _currentDirection = (_barnInterior.transform.position - transform.position).normalized;
            if ((_barnInterior.transform.position - transform.position).magnitude < 2.0f) {
                _currentState = AIState.IsGrazing;
            }
        }
    
        private void PickUpAndDropStuff() {
            if (!_hasPickUpAble) {
                var tempMagnitude = (_targetPickUpAble.transform.position - transform.position).magnitude;
                if (tempMagnitude < 2.0f) {
                    _targetPickUpAble.transform.position = transform.position + new Vector3(0.0f, _targetPickUpAble.transform.localScale.y, 0.0f);
                    _targetPickUpAble.transform.parent = transform;
                    var tempCurrentPickUpAbleRb = _targetPickUpAble.GetComponent<Rigidbody>();
                    tempCurrentPickUpAbleRb.useGravity = false;
                    tempCurrentPickUpAbleRb.isKinematic = true;
                    _hasPickUpAble = true;
                    _currentState = AIState.IsTravelingToBarnEntrance;
                }
            }
            else {
                // TODO: Maybe replace this with a boolean?
                if (_currentState == AIState.IsGrazing) {
                    var tempCurrentPickUpAbleRb = _targetPickUpAble.GetComponent<Rigidbody>();
                    tempCurrentPickUpAbleRb.useGravity = true;
                    tempCurrentPickUpAbleRb.isKinematic = false;
                    _targetPickUpAble.transform.localPosition += new Vector3(0.0f, 0.0f, _targetPickUpAble.transform.localScale.z);
                    _targetPickUpAble.transform.parent = null;
                    _targetPickUpAble.GetComponent<PickUpAbleBehavior>().HasFollowerDictionary[AnimalType] = false;
                    _targetPickUpAble = null;
                    _hasPickUpAble = false;
                }
            }
        }

        private void PickRandomDirection() {
            _timer -= Time.deltaTime;
            if (_timer < 0.0f) {
                _currentDirection = new Vector3(Random.Range(-1, 2), 0.0f, Random.Range(-1, 2)).normalized;
                _timer = Random.Range(1.0f, 3.0f);
            }
        }
    }
}
