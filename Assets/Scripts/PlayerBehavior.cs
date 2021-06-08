using UnityEngine;

public class PlayerBehavior : MonoBehaviour {

    private Rigidbody _rigidbody;

    private const float Speed = 1.0f;

    private void Start() {
        _rigidbody = GetComponent<Rigidbody>();
    }
    
    private void FixedUpdate() {
        _rigidbody.MovePosition(_rigidbody.position + new Vector3(-Input.GetAxisRaw("Vertical") * Speed * Time.deltaTime, 0.0f,
            Input.GetAxisRaw("Horizontal") * Speed * Time.deltaTime));
    }
}
