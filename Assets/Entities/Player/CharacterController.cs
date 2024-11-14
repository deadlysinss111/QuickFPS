using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;
using Unity.Netcode;
using System;
using System.Collections;
using TMPro;


public class CharacterController : NetworkBehaviour
{
    private Animator _animator;
    QuickFPS _pInput;
    float _originalMoveSpeed = 5f;
    float _runMoveSpeed = 7f;
    Vector3 _originalScale;

    NetworkVariable<float> _health = new(100f);
    [SerializeField] float _maxHealth = 100f;
    Image _damageImage;
    [SerializeField]  private DamageEffect _damageEffect;
    private bool _isGrounded;

    private BoxCollider _boxCollider;
    private Vector3 _originalSize;
    private Vector3 _originalCenter;
    private Vector3 _crouchSize = new Vector3(1, 2f, 1);
    private Vector3 _crouchCenterOffset = new Vector3(0, -1f, 0);

    private bool _isDead = false;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private GameObject _heart;
    [SerializeField] private Material _heartMaterial;

    Weapon _equipedWeapon;

    [SerializeField] LayerMask _layerMask;
    [SerializeField] Camera _camera;
    [SerializeField] public Transform _handSpot;

    [SerializeField] public TextMeshProUGUI _timeInSeconds;
    [SerializeField] public TextMeshProUGUI _timeInMinuts;
    [SerializeField] public TextMeshProUGUI _score;

    [NonSerialized] public NetworkVariable<int> _playerId = new(-1);

    private GameManager _gameManager;


    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _pInput = new QuickFPS();
        _equipedWeapon = null;
        _originalScale = transform.localScale;
        _boxCollider = GetComponent<BoxCollider>();
        if (_boxCollider != null)
        {
            _originalSize = _boxCollider.size;
            _originalCenter = _boxCollider.center;
        }
    }


    public override void OnNetworkSpawn()
    {
        gameOverUI.SetActive(false);
        _animator = GetComponent<Animator>();
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

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

    void OnTriggerEnter(Collider other)
    {
        if(!IsOwner) return;

        if (other.CompareTag("Ground"))
        {
            _isGrounded = true;
            SetAnimBoolRpc("isJumping", false);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            _isGrounded = false;
        }
    }

    void Update()
    {
        if (!IsOwner) return;

        if (!_isDead)
        {
            MovePlayer(_originalMoveSpeed);
        }
        if (_heart != null)
        {
            UpdateShader();
        }
        SetTexts();
    }

    private void SetTexts()
    {
        float time = GameManager._timeLeft;
        int seconds = (int)time % 60;
        int minuts = (int)((time - seconds) / 60);
        _timeInMinuts.SetText(minuts.ToString());
        _timeInSeconds.SetText(seconds.ToString());

        if(_playerId.Value == 0)
        {
            _score.SetText(_gameManager._player1score.Value.ToString());
        }
        else if(_playerId.Value == 1)
        {
            _score.SetText(_gameManager._player2score.Value.ToString());
        }
    }

    [Rpc(SendTo.Server)]
    private void SetAnimBoolRpc(string name, bool value)
    {
        _animator.SetBool(name, value);
    }

    private void MovePlayer(float speed)
    {
        Vector2 movementInput = _pInput.Player.Move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * movementInput.x + transform.forward * movementInput.y;

        if (movementInput != Vector2.zero)
        {
            SetAnimBoolRpc("isMoving", true);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.2f, _layerMask))
            {
                Vector3 slopeDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                transform.Translate(slopeDirection * speed * Time.deltaTime, Space.World);
            }
            else
            {
                transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
            }
        }
        else
        {
            SetAnimBoolRpc("isMoving", false);
        }
    }


    void Run(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            SetAnimBoolRpc("isRunning", true);
            _originalMoveSpeed = _runMoveSpeed;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            SetAnimBoolRpc("isRunning", false);
            _runMoveSpeed = _originalMoveSpeed;
            _originalMoveSpeed = 5f;
        }
    }

    void Jump(InputAction.CallbackContext context)
    {
        SetAnimBoolRpc("isJumping", true);
        if (_isGrounded)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
                _isGrounded = false;
            }
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
        if (_boxCollider == null) return;

        if (context.phase == InputActionPhase.Performed)
        {
            SetAnimBoolRpc("IsCrouched", true);
            _boxCollider.size = _crouchSize;
            _boxCollider.center = _originalCenter + _crouchCenterOffset;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            SetAnimBoolRpc("IsCrouched", false);
            _boxCollider.size = _originalSize;
            _boxCollider.center = _originalCenter;
        }
    }


    public void TakeDamage(float damage)
    {

        TakeDamageRpc(damage);

        
        //if (!IsOwner) return;
        
        _damageEffect.ShowDamageEffect();

        if (_health.Value <= 0)
        {
            _gameManager.IncrementScoreRpc(dmgFrom);
            _isDead = true;
            //ShowGameOverScreen();
            if(_equipedWeapon != null)
                _equipedWeapon.Drop();
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
