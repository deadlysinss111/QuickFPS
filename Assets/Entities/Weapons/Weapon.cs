using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    enum State
    {
        Grounded,
        Taken,
        Reloading,
    }

    State _state;
    QuickFPS _pInput;
    [SerializeField] int _mxAmmo;
    int _currentAmmo;
    [SerializeField] float _reloadingTime;

    [SerializeField] protected GameObject _bulletPrefab;

    private void Awake()
    {
        _pInput = new();
        _state = State.Grounded;
    }

    public void TakeInHand()
    {
        _pInput.Enable();
        _pInput.Player.Fire.performed += TriggerFire;
        _state = State.Taken;
    }

    public void Drop()
    {
        _pInput.Enable();
        _pInput.Player.Fire.performed -= TriggerFire;
        _state = State.Grounded;
    }

    private void TriggerFire(InputAction.CallbackContext ctx)
    {
        Fire();
    }

    protected void Fire() 
    {
        if (_currentAmmo > 0)
        {
            GameObject bullet = Instantiate(_bulletPrefab, transform.position + transform.rotation * new Vector3(2, 0, 0), transform.rotation);
            //Bullet bulletScript = bullet.GetComponent<Bullet>();
            --_currentAmmo;
        }
        else
        {
            Reload();
        }
        
        
    }

    protected void Reload()
    {
        StartCoroutine(ReloadingCorouine());
        _state = State.Reloading;
    }

    protected IEnumerator ReloadingCorouine()
    {
        yield return new WaitForSeconds(_reloadingTime);

        if (_state != State.Grounded)
        {
            _state = State.Taken;
            _currentAmmo = _mxAmmo;
        }
    }
}
