using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HitscanBullet : NetworkBehaviour
{
    [SerializeField] int _damage = 10;
    [SerializeField] int _dmgFalloff = 0;
    [SerializeField] GameObject _hitEffect;

    public void Trigger(Vector3 point, Vector3 normal)
    {
        //if (hit.transform.TryGetComponent(out Weapon _))
        //{
        //    print("hit");
        //    return;
        //}
        //if (hit.transform.TryGetComponent(out FollowPlayer enemy))
        //{
        //    //Hit(enemy);
        //}
        //else if (hit.transform.TryGetComponent(out CharacterController player))
        //{
        //    //Hit(player);
        //}

        HitEffect(point, normal);
        DestroySelfRpc();
    }

    virtual protected void HitEffect(Vector3 point, Vector3 normal)
    {
        SpawnEffectRpc(point, normal);
    }

    [Rpc(SendTo.Server)]
    private void SpawnEffectRpc(Vector3 point, Vector3 normal)
    {
        GameObject hole = Instantiate(_hitEffect, point + Vector3.Normalize(normal) * 0.01f, Quaternion.LookRotation(normal));
        //hole.transform.SetParent(transform);
        hole.GetComponent<NetworkObject>().SpawnWithOwnership(0, true);
    }

    [Rpc(SendTo.Server)]
    private void DestroySelfRpc()
    {
        Destroy(gameObject);
    }
}
