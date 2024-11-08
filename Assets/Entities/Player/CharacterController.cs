using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private QuickFPS _pInput;
    private float _originalMoveSpeed = 5f; 
    private float _runMoveSpeed = 10f; // Vitesse de déplacement du joueur
    public bool _isGrounded;
    private Vector3 _originalScale;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _pInput = new QuickFPS();
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        _pInput.Enable();
        _pInput.Player.Run.performed += Run;
        _pInput.Player.Run.canceled += Run;
        _pInput.Player.Jump.performed += Jump;
        _pInput.Player.Crouch.performed += Crouch;
        _pInput.Player.Crouch.canceled += Crouch;
    }

    private void OnDisable()
    {
        _pInput.Player.Run.performed -= Run;
        _pInput.Player.Run.canceled -= Run;
        _pInput.Player.Jump.performed -= Jump;
        _pInput.Player.Crouch.performed -= Crouch;
        _pInput.Player.Crouch.canceled -= Crouch;
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

    void Update()
    {
        MovePlayer(_originalMoveSpeed);
    }
    private void MovePlayer(float speed)
    {
        Vector2 movementInput = _pInput.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;
        moveDirection.y = 0;

        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void Run(InputAction.CallbackContext context)
    { 
        if (context.phase == InputActionPhase.Performed)
            _originalMoveSpeed = _runMoveSpeed;
        else if (context.phase == InputActionPhase.Canceled)
        {
            _runMoveSpeed = _originalMoveSpeed;
            _originalMoveSpeed = 5f;
        }
    }

    void Jump(InputAction.CallbackContext context)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && _isGrounded)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }

    void Crouch(InputAction.CallbackContext context)
    {
        if (_isGrounded)
        {
            if (context.phase == InputActionPhase.Performed)
            {
                Vector3 scale = transform.localScale;
                scale.y = _originalScale.y * 0.5f;
                transform.localScale = scale;
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                transform.localScale = _originalScale;
            }
        }
    }
}
