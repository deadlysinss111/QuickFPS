using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class CharacterController : MonoBehaviour
{
    QuickFPS _pInput;
    float _originalMoveSpeed = 5f;
    float _runMoveSpeed = 7f;
    bool _isGrounded;
    Vector3 _originalScale;

    public float _health = 100f;
    public Image _damageImage;

    private bool _isDead = false;
    [SerializeField] private GameObject gameOverUI;


    Weapon _equipedWeapon;

    [SerializeField] LayerMask _layerMask;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _pInput = new QuickFPS();
        _equipedWeapon = null;
        _originalScale = transform.localScale;
    }

    private void OnEnable()
    {
        _pInput.Enable();
        _pInput.Player.Run.performed += Run;
        _pInput.Player.Run.canceled += Run;
        _pInput.Player.Jump.performed += Jump;
        _pInput.Player.TakeWeapon.performed += TakeWeapon;
        _pInput.Player.DropWeapon.performed += DropWeapon;
        _pInput.Player.Crouch.performed += Crouch;
        _pInput.Player.Crouch.canceled += Crouch;
        //_pInput.Player.SelfDamage.performed += SelfDamage;
    }

    private void OnDisable()
    {
        _pInput.Player.Run.performed -= Run;
        _pInput.Player.Run.canceled -= Run;
        _pInput.Player.Jump.performed -= Jump;
        _pInput.Player.TakeWeapon.performed -= TakeWeapon;
        _pInput.Player.DropWeapon.performed -= DropWeapon;
        _pInput.Player.Crouch.performed -= Crouch;
        _pInput.Player.Crouch.canceled -= Crouch;
        //_pInput.Player.SelfDamage.performed -= SelfDamage;
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
        if (!_isDead)
        {
            MovePlayer(_originalMoveSpeed);
        }
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

    void TakeWeapon(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Transform camera = GetComponentInChildren<CameraPlayer>().transform;
        Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, _layerMask);
        if (hit.transform == null) return;

        Weapon _weaponScript;
        if (hit.transform.gameObject.TryGetComponent(out _weaponScript))
        {
            if(_equipedWeapon != null)
            {
                _equipedWeapon.Drop();
            }
            _weaponScript.TakeInHand();
            _weaponScript._handSpot = GameObject.Find("HandSpot").transform;
            hit.transform.SetParent(transform);
            _equipedWeapon = _weaponScript;
        }
    }

    void DropWeapon(InputAction.CallbackContext context)
    {
        if (_equipedWeapon != null)
        {
            _equipedWeapon.Drop();
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
    void SelfDamage(InputAction.CallbackContext context)
    {
        TakeDamage(10);
    }
    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        { 
            _isDead = true;
            ShowGameOverScreen();
        }
    }

    private void ShowGameOverScreen()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit();
    }

}

