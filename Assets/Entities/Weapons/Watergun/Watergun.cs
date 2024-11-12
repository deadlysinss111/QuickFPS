using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Watergun : Weapon
{
    bool _isShooting;
    [SerializeField] private float _shootCooldown;
    private float _remainingCooldown;

    protected override void Awake()
    {
        base.Awake();
        _isShooting = false;
        _remainingCooldown = _shootCooldown;
    }

    public override void TakeInHand(Transform handSpot, Transform camera, Transform playerransform, int ownerId)
    {
        base.TakeInHand(handSpot, camera, playerransform, ownerId);
        _pInput.Player.Fire.canceled += TriggerFireTravel;
    }

    public override void Drop()
    {
        base.Drop();
        _pInput.Player.Fire.canceled -= TriggerFireTravel;
        _isShooting = false;
    }

    protected override void TriggerFireTravel(InputAction.CallbackContext ctx)
    {
        _isShooting = !_isShooting;
    }

    protected override void Update()
    {
        base.Update();
        if(_isShooting)
        {
            LookForFiring();
        }
    }

    private void LookForFiring()
    {
        if (!_isShooting) return;

        _remainingCooldown -= Time.deltaTime;
        if(_remainingCooldown <= 0)
        {
            FireTravel();
        }
    }
}
