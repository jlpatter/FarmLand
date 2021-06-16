using System;
using System.Linq;
using AnimalBehavior;
using Cinemachine;
using StartMenu;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    public AnimalTypes AnimalType { get; private set; }
    public GameManagerBehavior GameManagerBehavior { get; private set; }
    public float Health { get; set; }
    private float Speed { get; set; }

    public GameObject rabbitPrefab;
    public GameObject cowPrefab;
    public GameObject pigPrefab;
    // TODO: Add chicken prefab.
    
    public GameObject sword;
    public GameObject axe;
    public Transform groundCheckTransform;
    public LayerMask groundMask;
    
    private CharacterController _characterController;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _hasPickUpAble;
    private GameObject _currentPickUpAble;
    private PlayerHealthBar _healthBar;

    private const float TurnSmoothTime = 0.1f;
    private const float Gravity = -9.81f;
    private const float GroundDistance = 0.4f;
    private const float TrampleStrength = 1.0f;

    private void Start() {
        Cursor.visible = false;
        _characterController = GetComponent<CharacterController>();
        _hasPickUpAble = false;
        _currentPickUpAble = null;
        _healthBar = GameObject.Find("HealthBar").GetComponent<PlayerHealthBar>();
        switch (StartMenuValue.animal) {
            case 0:
                AnimalType = AnimalTypes.Rabbit;
                transform.position = GameObject.Find(AnimalType + "Spawner").transform.position;
                Instantiate(rabbitPrefab, transform.position, Quaternion.Euler(-90.0f, 0.0f, 90.0f), transform);
                break;
            case 1:
                AnimalType = AnimalTypes.Cow;
                transform.position = GameObject.Find(AnimalType + "Spawner").transform.position;
                Instantiate(cowPrefab, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f), transform);
                
                var boxCollider = gameObject.AddComponent<BoxCollider>();
                boxCollider.isTrigger = true;
                boxCollider.center = new Vector3(0.03f, -0.63f, 0.0f);
                boxCollider.size = new Vector3(0.7f, 1.94f, 1.85f);
                
                groundCheckTransform.localPosition = new Vector3(groundCheckTransform.localPosition.x, -0.635f, groundCheckTransform.localPosition.z);
                _characterController.center = new Vector3(_characterController.center.x, 0.06f, _characterController.center.z);
                _characterController.radius = 0.32f;
                _characterController.height = 1.15f;
                break;
            case 2:
                AnimalType = AnimalTypes.Pig;
                transform.position = GameObject.Find(AnimalType + "Spawner").transform.position;
                Instantiate(pigPrefab, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f), transform);
                groundCheckTransform.localPosition = new Vector3(groundCheckTransform.localPosition.x, -0.359f, groundCheckTransform.localPosition.z);
                _characterController.center = new Vector3(_characterController.center.x, 0.09f, _characterController.center.z);
                _characterController.radius = 0.17f;
                _characterController.height = 0.63f;
                break;
            case 3:
                AnimalType = AnimalTypes.Chicken;
                // TODO: Insert prefab here!
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        GameManagerBehavior = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
        GameManagerBehavior.AllAnimals.Add(new Tuple<GameObject, AnimalTypes>(gameObject, AnimalType));
        Health = GameManagerBehavior.AnimalAttributesDict[AnimalType].Health;
        var healthBar = GameObject.Find("HealthBar").GetComponent<PlayerHealthBar>();
        healthBar.SetMaxHealth(Health);
        healthBar.SetHealth(Health);
        Speed = GameManagerBehavior.AnimalAttributesDict[AnimalType].Speed;
    }

    private void Update() {
        MovePlayer();
        SwingWeapon();
        PickUpAndDropStuff();
    }

    private void OnTriggerEnter(Collider other) {
        if (AnimalType == AnimalTypes.Cow) {
            TrampleEnemies(other);
        }
        
        if (other.name.Equals("Weapon")) {
            RemoveHealth(GameManagerBehavior.SwordDamage);
        }

        if (other.name.Contains("Axe")) {
            RemoveHealth(GameManagerBehavior.AxeDamage);
        }
    }

    private void OnTriggerStay(Collider other) {
        if (AnimalType == AnimalTypes.Cow) {
            TrampleEnemies(other);
        }
    }

    private void RemoveHealth(float weaponDamage) {
        Health -= weaponDamage;
        _healthBar.SetHealth(Health);

        if (Health <= 0.0f) {
            var toRemoveList = GameManagerBehavior.AllAnimals.Where(tuple => gameObject == tuple.Item1).ToList();
            foreach (var removeMe in toRemoveList) {
                GameManagerBehavior.AllAnimals.Remove(removeMe);
            }

            PickUpAbleBehavior.DeParent(gameObject, AnimalType);
            Destroy(gameObject);
        }
    }

    private void MovePlayer() {
        _isGrounded = Physics.CheckSphere(groundCheckTransform.position, GroundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0) {
            _velocity.y = -2.0f;
        }
        
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        if (direction.magnitude >= 0.1f) {

            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            var moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            _characterController.Move(moveDirection.normalized * (Speed * Time.deltaTime));
        }

        _velocity.y += Gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void SwingWeapon() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (AnimalType == AnimalTypes.Rabbit) {
                sword.SetActive(true);
            }
            else if (AnimalType == AnimalTypes.Pig) {
                axe.SetActive(true);
            }
        }
    }

    private void TrampleEnemies(Collider other) {
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
    }

    private void PickUpAndDropStuff() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (!_hasPickUpAble) {
                var allPickUpAbles = GameObject.FindGameObjectsWithTag("PickUpAble");
                foreach (var pickUpAble in allPickUpAbles) {
                    var tempMagnitude = (pickUpAble.transform.position - transform.position).magnitude;
                    if (tempMagnitude < 2.0f) {
                        _currentPickUpAble = pickUpAble;
                        _hasPickUpAble = true;
                        break;
                    }
                }
                if (_hasPickUpAble) {
                    _currentPickUpAble.transform.position = transform.position + new Vector3(0.0f, _currentPickUpAble.transform.localScale.y, 0.0f);
                    _currentPickUpAble.transform.parent = transform;
                    _currentPickUpAble.GetComponent<PickUpAbleBehavior>().HasFollowerDictionary[AnimalType] = false;
                    var tempCurrentPickUpAbleRb = _currentPickUpAble.GetComponent<Rigidbody>();
                    tempCurrentPickUpAbleRb.useGravity = false;
                    tempCurrentPickUpAbleRb.isKinematic = true;
                }
            }
            else {
                var tempCurrentPickUpAbleRb = _currentPickUpAble.GetComponent<Rigidbody>();
                tempCurrentPickUpAbleRb.useGravity = true;
                tempCurrentPickUpAbleRb.isKinematic = false;
                _currentPickUpAble.transform.localPosition += new Vector3(0.0f, 0.0f, _currentPickUpAble.transform.localScale.z);
                _currentPickUpAble.transform.parent = null;
                _currentPickUpAble = null;
                _hasPickUpAble = false;
            }
        }
    }
}
