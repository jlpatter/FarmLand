using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    public GameObject weapon;
    public Transform cameraTransform;
    public Transform groundCheckTransform;
    public LayerMask groundMask;
    
    private CharacterController _characterController;
    private float _turnSmoothVelocity;
    private Vector3 _velocity;
    private bool _isGrounded;

    private const float Speed = 6.0f;
    private const float TurnSmoothTime = 0.1f;
    private const float Gravity = -9.81f;
    private const float GroundDistance = 0.4f;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update() {
        MovePlayer();
        SwingWeapon();
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
}
