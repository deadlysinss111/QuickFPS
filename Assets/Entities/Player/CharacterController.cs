using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private QuickFPS _pInput;
    public float moveSpeed = 5f;
    public bool _isGrounded;

    void Awake()
    {
        _pInput = new QuickFPS();
    }

    private void OnEnable()
    {
        _pInput.Enable();
        _pInput.Player.Jump.performed += Jump;
    }

    private void OnDisable()
    {
        _pInput.Player.Jump.performed -= Jump;
        _pInput.Disable();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 movementInput = _pInput.Player.Move.ReadValue<Vector2>();

        Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;
        moveDirection.y = 0;

        transform.Translate(moveDirection * _moveSpeed * Time.deltaTime, Space.World);
    }

    void Jump(InputAction.CallbackContext context)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && _isGrounded) // V�rifie que le joueur est au sol
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }
}
