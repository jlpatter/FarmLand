using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIBehavior : MonoBehaviour {
    public Transform groundCheckTransform;
    public LayerMask groundMask;

    private Animal _animal;
    private AIState _currentState;
    private CharacterController _characterController;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _timer;
    private Vector3 _currentDirection;
    private GameObject _playerGameObject;
    private GameObject _targetPickUpAble;
    private GameObject _barnEntrance;
    private GameObject _barnInterior;
    private bool _hasPickUpAble;

    protected float speed = 5.0f;
    
    private const float TurnSmoothTime = 0.1f;
    private const float Gravity = -9.81f;
    private const float GroundDistance = 0.4f;

    private enum AIState {
        IsGrazing,
        FindingPickUpAble,
        FollowPlayer,
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
        _playerGameObject = GameObject.Find("Player");

        if (name.Contains("Rabbit")) {
            _animal = Animal.Rabbit;
        }
        else if (name.Contains("Cow")) {
            _animal = Animal.Cow;
        }
        else if (name.Contains("Pig")) {
            _animal = Animal.Pig;
        }
        else if (name.Contains("Chicken")) {
            _animal = Animal.Chicken;
        }

        _barnInterior = GameObject.Find(_animal + "Barn").transform.Find("Middle").gameObject;
        _barnEntrance = GameObject.Find(_animal + "Barn").transform.Find("Entrance").gameObject;
    }

    private void Update() {

        _isGrounded = Physics.CheckSphere(groundCheckTransform.position, GroundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0) {
            _velocity.y = -2.0f;
        }

        switch (_currentState) {
            case AIState.IsGrazing:
                PickRandomDirection();
                FindPlayer();
                break;
            case AIState.FindingPickUpAble:
                FindPickUpAble();
                break;
            case AIState.FollowPlayer:
                FollowPlayer();
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
            _characterController.Move(moveDirection.normalized * (speed * Time.deltaTime));
        }

        _velocity.y += Gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.name.Equals("Weapon")) {
            Destroy(gameObject);
        }
    }

    private void FindPickUpAble() {
        var pickUpAbles = GameObject.FindGameObjectsWithTag("PickUpAble");
        foreach (var pickUpAble in pickUpAbles) {
            var pickUpAbleBehavior = pickUpAble.GetComponent<PickUpAbleBehavior>();
            if (!pickUpAbleBehavior.HasFollower) {
                _targetPickUpAble = pickUpAble;
                _currentState = AIState.IsTravelingToPickUpAble;
                pickUpAbleBehavior.HasFollower = true;
                break;
            }
        }

        if (_targetPickUpAble == null) {
            _currentState = AIState.IsGrazing;
        }
    }

    private void FindPlayer() {
        if ((_playerGameObject.transform.position - transform.position).magnitude < 10.0f && _playerGameObject.GetComponent<PlayerBehavior>().Animal != _animal) {
            _currentState = AIState.FollowPlayer;
        }
    }

    private void FollowPlayer() {
        _currentDirection = (_playerGameObject.transform.position - transform.position).normalized;
        if ((_playerGameObject.transform.position - transform.position).magnitude > 10.0f) {
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
                _targetPickUpAble.GetComponent<PickUpAbleBehavior>().HasFollower = false;
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
