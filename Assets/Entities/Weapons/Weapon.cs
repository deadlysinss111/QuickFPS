using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Weapon : NetworkBehaviour
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
    [NonSerialized] public Transform _cameraToFollow;
    [NonSerialized] public Transform _playerTrasform;

    private NetworkVariable<int> _ownerID = new(-1);
    private NetworkVariable<Vector3> _framePosition = new();
    private NetworkVariable<Quaternion> _frameRotation = new();

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

    virtual public void TakeInHand(Transform handSpot, Transform camera, Transform playerransform, int playerId)
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

        _handSpot = handSpot;
        _cameraToFollow = camera;
        _playerTrasform = playerransform;
        SetOwnerIDRpc(playerId);

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
        

        GetComponent<Rigidbody>().constraints = 0;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().AddForce(Vector3.Normalize(_cameraToFollow.transform.forward)*500);

        _handSpot = null;
        _cameraToFollow = null;
        _playerTrasform  = null;
        SetOwnerIDRpc(-1);


        GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText("-");
        GameObject.Find("AmmoMax").GetComponent<TextMeshProUGUI>().SetText("-");
    }

    [Rpc(SendTo.Server)]
    private void SetOwnerIDRpc(int value)
    {
        _ownerID.Value = value;
        
    }

    virtual protected void Update()
    {
        if (_state != State.Grounded)
        {
            Vector3 eulerCamera = _cameraToFollow.rotation.eulerAngles;
            Vector3 eulerPlyer = _playerTrasform.rotation.eulerAngles;
            UpdateValuesRpc(_handSpot.position, Quaternion.Euler(eulerCamera.x, eulerPlyer.y, 0));
            ApplyValuesRpc();
        }
        
    }

    [Rpc(SendTo.Server)]
    private void UpdateValuesRpc(Vector3 position, Quaternion Rotation)
    {
        _framePosition.Value = position;
        _frameRotation.Value = Rotation;
    }

    [Rpc(SendTo.Server)]
    private void ApplyValuesRpc()
    {
        transform.position = _framePosition.Value;
        transform.localRotation = _frameRotation.Value;
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
                SpawnHitScanBullettRpc(hit.point, hit.normal);
                
                
            }
            --CurrentAmmo;
        }
        else
        {
            Reload();
            GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText("-");
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnHitScanBullettRpc(Vector3 point, Vector3 normal)
    {
        GameObject bullet = Instantiate(_bulletPrefab);
        bullet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
        bullet.GetComponent<HitscanBullet>().Trigger(point, normal);
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
            SpawnTravelBullettRpc(tip.position, _cameraToFollow.transform.rotation * Quaternion.Euler(0, -90, 0));

            //Bullet bulletScript = bullet.GetComponent<Bullet>();
            --CurrentAmmo;
        }
        else
        {
            Reload();
            GameObject.Find("AmmoLeft").GetComponent<TextMeshProUGUI>().SetText("-");
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnTravelBullettRpc(Vector3 origin, Quaternion rotation)
    {
        
        GameObject bullet = Instantiate(_bulletPrefab, origin, rotation);
        bullet.GetComponent<NetworkObject>().SpawnWithOwnership(OwnerClientId, true);
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
