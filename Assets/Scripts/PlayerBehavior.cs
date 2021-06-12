using System;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    public Animal Animal { get; private set; }
    public GameManagerBehavior GameManagerBehavior { get; private set; }
    
    public GameObject weapon;
    public Transform cameraTransform;
    public Transform groundCheckTransform;
    public LayerMask groundMask;
    
    private CharacterController _characterController;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _hasPickUpAble;
    private GameObject _currentPickUpAble;

    private const float Speed = 6.0f;
    private const float TurnSmoothTime = 0.1f;
    private const float Gravity = -9.81f;
    private const float GroundDistance = 0.4f;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
        _hasPickUpAble = false;
        _currentPickUpAble = null;
        Animal = Animal.Rabbit;
        
        GameManagerBehavior = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
        GameManagerBehavior.AllAnimals.Add(new Tuple<GameObject, Animal>(gameObject, Animal));
    }

    private void Update() {
        MovePlayer();
        SwingWeapon();
        PickUpAndDropStuff();
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

            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
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
            weapon.SetActive(true);
        }
    }

    private void PickUpAndDropStuff() {
        if (Input.GetKeyDown(KeyCode.E)) {
            if (!_hasPickUpAble) {
                var allPickUpAbles = GameObject.FindGameObjectsWithTag("PickUpAble");
                var currentMagnitude = float.PositiveInfinity;
                foreach (var pickUpAble in allPickUpAbles) {
                    var tempMagnitude = (pickUpAble.transform.position - transform.position).magnitude;
                    if (tempMagnitude < currentMagnitude && tempMagnitude < 2.0f) {
                        _currentPickUpAble = pickUpAble;
                        _hasPickUpAble = true;
                        currentMagnitude = tempMagnitude;
                    }
                }
                if (_hasPickUpAble) {
                    _currentPickUpAble.transform.position = transform.position + new Vector3(0.0f, _currentPickUpAble.transform.localScale.y, 0.0f);
                    _currentPickUpAble.transform.parent = transform;
                    _currentPickUpAble.GetComponent<PickUpAbleBehavior>().HasFollowerDictionary[Animal] = false;
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
