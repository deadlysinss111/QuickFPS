using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private QuickFPS _pInput;
    public Transform cameraTransform; // R�f�rence � la cam�ra enfant�e au joueur
    public float mouseSensitivity = 100f; // Sensibilit� de la souris
    public float moveSpeed = 5f; // Vitesse de d�placement du joueur

    private float xRotation = 0f; // Pour limiter la rotation verticale

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
        // Verrouiller le curseur pour un contr�le FPS
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        MovePlayer();
    }
    private void MovePlayer()
    {
        Vector2 movementInput = _pInput.Player.Move.ReadValue<Vector2>();

        // Calculer le d�placement en fonction de la direction du joueur
        Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;
        moveDirection.y = 0; // Emp�che le joueur de se d�placer verticalement

        // Appliquer le mouvement avec la vitesse d�finie
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    void Jump(InputAction.CallbackContext context)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null && rb.tag == "isGrounded") // V�rifie que le joueur est au sol
        {
            rb.AddForce(Vector3.up * 100, ForceMode.Impulse);
        }
    }
}
