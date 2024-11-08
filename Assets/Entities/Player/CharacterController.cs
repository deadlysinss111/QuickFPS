using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private QuickFPS _pInput;
    public float moveSpeed = 5f; // Vitesse de déplacement du joueur
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
        // Verrouiller le curseur pour un contrôle FPS
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
    }
    private void MovePlayer()
    {
        Vector2 movementInput = _pInput.Player.Move.ReadValue<Vector2>();

        // Calculer le déplacement en fonction de la direction du joueur
        Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;
        moveDirection.y = 0; // Empêche le joueur de se déplacer verticalement

        // Appliquer le mouvement avec la vitesse définie
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void Jump(InputAction.CallbackContext context)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && _isGrounded) // Vérifie que le joueur est au sol
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }
}
