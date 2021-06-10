using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBehavior : MonoBehaviour {
    public Transform groundCheckTransform;
    public LayerMask groundMask;
    
    private CharacterController _characterController;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private bool _isGrounded;
    private float _timer;
    private Vector3 _currentDirection;

    private const float Speed = 2.0f;
    private const float TurnSmoothTime = 0.1f;
    private const float Gravity = -9.81f;
    private const float GroundDistance = 0.4f;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
        _timer = -1.0f;
    }

    private void Update() {

        _isGrounded = Physics.CheckSphere(groundCheckTransform.position, GroundDistance, groundMask);

        if (_isGrounded && _velocity.y < 0) {
            _velocity.y = -2.0f;
        }

        PickRandomDirection();
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

    private void PickRandomDirection() {
        _timer -= Time.deltaTime;
        if (_timer < 0.0f) {
            _currentDirection = new Vector3(Random.Range(-1, 2), 0.0f, Random.Range(-1, 2));
            _timer = Random.Range(1.0f, 3.0f);
        }
    }
}
