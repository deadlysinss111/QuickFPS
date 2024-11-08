using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    private QuickFPS _pInput;

    [SerializeField] protected GameObject _bulletPrefab;

    private void Awake()
    {
        _pInput = new();
        TakeInHand();
    }

    public void TakeInHand()
    {
        _pInput.Enable();
        _pInput.Player.Fire.performed += TriggerFire;
    }

    public void Drop()
    {
        _pInput.Enable();
        _pInput.Player.Fire.performed -= TriggerFire;
    }

    private void TriggerFire(InputAction.CallbackContext ctx)
    {
        Fire();
    }

    virtual protected void Fire() 
    {
        GameObject bullet = Instantiate(_bulletPrefab, transform.position, transform.rotation);
        //Bullet bulletScript = bullet.GetComponent<Bullet>();
    }
}
