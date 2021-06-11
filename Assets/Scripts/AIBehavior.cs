using UnityEngine;
using Random = UnityEngine.Random;

public class AIBehavior : MonoBehaviour {
    public Transform groundCheckTransform;
    public LayerMask groundMask;

    private Animal _animal;
    private CharacterController _characterController;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _timer;
    private Vector3 _currentDirection;
    private bool _isGrazing;
    private GameObject _targetPickUpAble;
    private GameObject _barnEntrance;
    private GameObject _barnInterior;
    private bool _hasPickUpAble;
    private bool _goingToBarn;
    private bool _followBarnInterior;
    private bool _isInBarn;

    private const float Speed = 2.0f;
    private const float TurnSmoothTime = 0.1f;
    private const float Gravity = -9.81f;
    private const float GroundDistance = 0.4f;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
        _timer = -1.0f;
        _isGrazing = false;
        _targetPickUpAble = null;
        _hasPickUpAble = false;
        _goingToBarn = false;
        _followBarnInterior = false;
        _isInBarn = false;
        
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

        if (_isGrazing) {
            PickRandomDirection();
        }
        else {
            if (_targetPickUpAble == null) {
                FindPickUpAble();
            }
            else {
                if (!_goingToBarn) {
                    FollowPickUpAble();
                }
                else {
                    if (_followBarnInterior) {
                        FollowBarnInterior();
                    }
                    else {
                        FollowBarn();
                    }
                }
                PickUpAndDropStuff();
            }
        }
        if (_currentDirection.magnitude >= 0.1f) {

            var targetAngle = Mathf.Atan2(_currentDirection.x, _currentDirection.z) * Mathf.Rad2Deg;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            var moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            _characterController.Move(moveDirection.normalized * (Speed * Time.deltaTime));
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
                _currentDirection = (pickUpAble.transform.position - transform.position).normalized;
                _targetPickUpAble = pickUpAble;
                pickUpAbleBehavior.HasFollower = true;
                break;
            }
        }

        if (_targetPickUpAble == null) {
            _isGrazing = true;
        }
    }

    private void FollowPickUpAble() {
        _currentDirection = (_targetPickUpAble.transform.position - transform.position).normalized;
    }

    private void FollowBarn() {
        _currentDirection = (_barnEntrance.transform.position - transform.position).normalized;
        if ((_barnEntrance.transform.position - transform.position).magnitude < 2.0f) {
            _followBarnInterior = true;
        }
    }

    private void FollowBarnInterior() {
        _currentDirection = (_barnInterior.transform.position - transform.position).normalized;
        if ((_barnInterior.transform.position - transform.position).magnitude < 2.0f) {
            _isInBarn = true;
            _isGrazing = true;
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
                _goingToBarn = true;
            }
        }
        else {
            if (_isInBarn) {
                var tempCurrentPickUpAbleRb = _targetPickUpAble.GetComponent<Rigidbody>();
                tempCurrentPickUpAbleRb.useGravity = true;
                tempCurrentPickUpAbleRb.isKinematic = false;
                _targetPickUpAble.transform.localPosition += new Vector3(0.0f, 0.0f, _targetPickUpAble.transform.localScale.z);
                _targetPickUpAble.transform.parent = null;
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
