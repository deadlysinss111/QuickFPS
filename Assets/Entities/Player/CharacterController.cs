using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private QuickFPS _pInput;

    public Transform _cameraTransform;
    public float _mouseSensitivity = 100f;
    private float _xRotation = 0f;

    public float _moveSpeed = 5f;

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
        if (rb != null && rb.tag == "isGrounded")
        {
            rb.AddForce(Vector3.up * 100, ForceMode.Impulse);
        }
    }
}
