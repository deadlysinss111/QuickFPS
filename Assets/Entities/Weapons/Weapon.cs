using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : MonoBehaviour
{
    protected enum Type
    {
        HitScan,
        Travel,
    }

    protected enum State
    {
        Grounded,
        Taken,
        Reloading,
    }


    protected State _state;
    protected QuickFPS _pInput;
    protected int _currentAmmo;
    int CurrentAmmo { 
        get => _currentAmmo; 
        set { _currentAmmo = value; GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText(_currentAmmo.ToString()); } }

    [SerializeField] protected int _maxAmmo;
    [SerializeField] protected float _reloadingTime;
    [SerializeField] protected Type _type = Type.HitScan;
    [SerializeField] protected GameObject _bulletPrefab;

    [NonSerialized] public Transform _handSpot;

    virtual protected void Awake()
    {
        _pInput = new();
        _state = State.Grounded;
        _currentAmmo = _maxAmmo;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Rigidbody>().useGravity = true;
    }


    // --- Behaviour as Game Object --- //
    #region GameObjectBehaviour

    virtual public void TakeInHand()
    {
        _pInput.Enable();

        switch (_type)
        {
            case Type.HitScan:
                _pInput.Player.Fire.performed += TriggerFireHitscan;
                break;
            case Type.Travel:
                _pInput.Player.Fire.performed += TriggerFireTravel;
                break;
        }

        _pInput.Player.Reload.performed += Reload;
        _state = State.Taken;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GetComponent<Rigidbody>().useGravity = false;

        GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText(_currentAmmo.ToString());
        GameObject.Find("AmmoMax").GetComponent<TextMeshProUGUI>().SetText(_maxAmmo.ToString());
    }

    virtual public void Drop()
    {
        switch (_type)
        {
            case Type.HitScan:
                _pInput.Player.Fire.performed -= TriggerFireHitscan;
                break;
            case Type.Travel:
                _pInput.Player.Fire.performed -= TriggerFireTravel;
                break;
        }
        
        _pInput.Player.Reload.performed -= Reload;
        _state = State.Grounded;
        _pInput.Disable();
        transform.SetParent(null);

        GetComponent<Rigidbody>().constraints = 0;
        GetComponent<Rigidbody>().useGravity = true;

        Transform camera = GameObject.Find("Main Camera").transform;
        GetComponent<Rigidbody>().AddForce(Vector3.Normalize(camera.transform.forward)*500);

        GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText("-");
        GameObject.Find("AmmoMax").GetComponent<TextMeshProUGUI>().SetText("-");
    }

    virtual protected void Update()
    {
        if (_state != State.Grounded)
        {
            transform.position = _handSpot.position;
            Vector3 eulerCamera = GameObject.Find("Main Camera").transform.rotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(0, 0, -eulerCamera.x);
        }
    }
    #endregion


    // --- Behaviour as Weapon --- //
    #region WeaponBehaviour

    virtual protected void TriggerFireHitscan(InputAction.CallbackContext ctx)
    {
        FireHitscan();
    }

    virtual protected void FireHitscan() 
    {
        if (_currentAmmo > 0)
        {
            RaycastHit hit;
            Transform camera = GameObject.Find("Main Camera").transform;
            Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, Mathf.Infinity, LayerMask.NameToLayer("Everything") - LayerMask.NameToLayer("Bullets"));
            if (hit.transform != null)
            {
                GameObject bullet = Instantiate(_bulletPrefab);
                bullet.GetComponent<HitscanBullet>().Trigger(hit);
            }
            --CurrentAmmo;
        }
        else
        {
            Reload();
            GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText("-");
        }
    }

    virtual protected void TriggerFireTravel(InputAction.CallbackContext ctx)
    {
        FireTravel();
    }

    virtual protected void FireTravel()
    {
        if (_currentAmmo > 0)
        {
            Transform tip = transform.Find("CanonTip");
            GameObject bullet = Instantiate(
                _bulletPrefab,
                tip.position,
                GameObject.Find("Main Camera").transform.rotation * Quaternion.Euler(0, -90, 0)
                );

            //Bullet bulletScript = bullet.GetComponent<Bullet>();
            --CurrentAmmo;
        }
        else
        {
            Reload();
            GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText("-");
        }
    }



    private void Reload(InputAction.CallbackContext ctx)
    {
        Reload();
    }
    virtual protected void Reload()
    {
        if (_state == State.Reloading) return;

        _currentAmmo = 0;
        GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText("-");
        StartCoroutine(ReloadingCorouine());
        _state = State.Reloading;
    }


    virtual protected IEnumerator ReloadingCorouine()
    {
        yield return new WaitForSeconds(_reloadingTime);

        if (_state != State.Grounded)
        {
            _state = State.Taken;
            CurrentAmmo = _maxAmmo;
            
        }
    }

    #endregion
}
