using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    public Transform cameraTransform;
    
    private CharacterController _characterController;
    private float _turnSmoothVelocity;

    private const float Speed = 6.0f;
    private const float TurnSmoothTime = 0.1f;

    private void Start() {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update() {
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        if (direction.magnitude >= 0.1f) {

            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0.0f, angle, 0.0f);

            var moveDirection = Quaternion.Euler(0.0f, targetAngle, 0.0f) * Vector3.forward;
            _characterController.Move(moveDirection.normalized * (Speed * Time.deltaTime));
        }
    }
}
