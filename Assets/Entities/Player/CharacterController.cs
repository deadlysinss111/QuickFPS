using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using Unity.Netcode;
using System;
using System.Collections;


public class CharacterController : NetworkBehaviour
{
    QuickFPS _pInput;
    float _originalMoveSpeed = 5f;
    float _runMoveSpeed = 7f;
    bool _isGrounded;
    Vector3 _originalScale;

    NetworkVariable<float> _health = new(100f);
    [SerializeField] float _maxHealth = 100f;
    Image _damageImage;
    [SerializeField]  private DamageEffect _damageEffect;

    private bool _isDead = false;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private GameObject _heart;
    [SerializeField] private Material _heartMaterial;

    Weapon _equipedWeapon;

    [SerializeField] LayerMask _layerMask;
    [SerializeField] Camera _camera;
    [SerializeField] public Transform _handSpot;

    [NonSerialized] public NetworkVariable<int> _playerId = new(-1);

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _pInput = new QuickFPS();
        _equipedWeapon = null;
        _originalScale = transform.localScale;
    }

    public override void OnNetworkSpawn()
    {
        gameOverUI.SetActive(false);
        if (!IsOwner)
        {
            _camera.gameObject.SetActive(false);
            transform.Find("CanvasCamera").gameObject.SetActive(false);
            transform.Find("Canvas").gameObject.SetActive(false);
            return;
        }

        _pInput.Enable();
        _pInput.Player.Run.performed += Run;
        _pInput.Player.Run.canceled += Run;
        _pInput.Player.Jump.performed += Jump;
        _pInput.Player.TakeWeapon.performed += TakeWeapon;
        _pInput.Player.DropWeapon.performed += DropWeapon;
        _pInput.Player.Crouch.performed += Crouch;
        _pInput.Player.Crouch.canceled += Crouch;

        transform.Find("Main Camera").GetComponent<AudioManager>().Init();
        transform.Find("Canvas").SetParent(null);
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
        if (!_isDead && IsOwner)
        {
            MovePlayer(_originalMoveSpeed);
        }
        if (_heart != null)
        {
            UpdateShader();
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
        Debug.DrawRay(camera.transform.position, camera.transform.forward, Color.green, 1000);
        Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, _layerMask);
        if (hit.transform == null) return;

        Weapon weaponScript;
        if (hit.transform.gameObject.TryGetComponent(out weaponScript))
        {
            if(_equipedWeapon != null)
            {
                _equipedWeapon.Drop();
            }
            weaponScript.TakeInHand(_handSpot, _camera.transform, transform, _playerId.Value);
            _equipedWeapon = weaponScript;
        }
    }


    void DropWeapon(InputAction.CallbackContext context)
    {
        if (_equipedWeapon != null)
        {
            _equipedWeapon.Drop();
        }
        _equipedWeapon = null;
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

    public void TakeDamage(float damage)
    {

        TakeDamageRpc(damage);

        if (!IsOwner) return;

        _damageEffect.ShowDamageEffect();

        if (_health.Value <= 0)
        {
            _isDead = true;
            //ShowGameOverScreen();
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        transform.position = new Vector3(500, 500, 500);
        yield return new WaitForSeconds(4);
        GameObject.Find("GameManager").GetComponent<GameManager>().RespawnPlayer(gameObject);
        _isDead = false;
    }

    [Rpc(SendTo.Server)]
    private void TakeDamageRpc(float damage)
    {
        _health.Value -= damage;
        if (_damageEffect != null)
        {
            _health.Value = Mathf.Clamp(_health.Value, 0, _maxHealth);
        }

        print(_health.Value);

        
    }

    private void ShowGameOverScreen()
    {
        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
        }
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Quit()
    {
        Application.Quit(); // For builded version
        //UnityEditor.EditorApplication.isPlaying = false; // for edited version
    }

    private void UpdateShader()
    {
        if (_heartMaterial != null && IsOwner)
        {
            float normalizedHP = Mathf.Clamp01(_health.Value / _maxHealth);
            _heartMaterial.SetFloat("_HP", normalizedHP);
        }
    }
    
    public Transform GetCamera()
    {
        return _camera.transform;
    }

    public void HealSelf()
    {
        HealRpc();
    }

    [Rpc(SendTo.Server)]
    private void HealRpc()
    {
        _health.Value = _maxHealth;
    }

}
