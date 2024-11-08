using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController : MonoBehaviour
{
    private QuickFPS _pInput;
    public float _moveSpeed = 5f;
    public bool _isGrounded;

    Weapon _equipedWeapon;

    [SerializeField] LayerMask _layerMask;

    void Awake()
    {
        _pInput = new QuickFPS();
        _equipedWeapon = null;
    }

    private void OnEnable()
    {
        _pInput.Enable();
        _pInput.Player.Jump.performed += Jump;
        _pInput.Player.TakeWeapon.performed += TakeWeapon;
        _pInput.Player.DropWeapon.performed += DropWeapon;
    }

    private void OnDisable()
    {
        _pInput.Player.Jump.performed -= Jump;
        _pInput.Player.TakeWeapon.performed -= TakeWeapon;
        _pInput.Player.DropWeapon.performed -= DropWeapon;
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
        if (rb != null && _isGrounded)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        }
    }

    void TakeWeapon(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Transform camera = GetComponentInChildren<CameraPlayer>().transform;
        Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, _layerMask);
        if (hit.transform == null) return;

        Weapon _weaponScript;
        if (hit.transform.gameObject.TryGetComponent(out _weaponScript))
        {
            print("sg");
            _weaponScript.TakeInHand();
            hit.transform.position = transform.position + new Vector3(1, 0, 0);
            hit.transform.SetParent(transform);
            _equipedWeapon = _weaponScript;
        }
    }

    void DropWeapon(InputAction.CallbackContext context)
    {
        if (_equipedWeapon != null)
        {
            _equipedWeapon.Drop();
            _equipedWeapon.transform.SetParent(null);
        }
    }
}
