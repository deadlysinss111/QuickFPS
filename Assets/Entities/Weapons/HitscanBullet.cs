using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HitscanBullet : NetworkBehaviour
{
    [SerializeField] int _damage = 1;
    [SerializeField] int _dmgFalloff = 0;
    [SerializeField] GameObject _hitEffect;

    public virtual void Hit(FollowPlayer enemy)
    {
        Hit((MonoBehaviour)enemy);
        enemy.GetComponent<FollowPlayer>().TakeDamage(_damage, OwnerClientId);
    }
    public virtual void Hit(CharacterController player)
    {
        Hit((MonoBehaviour)player);
        player.TakeDamage(_damage, OwnerClientId);

        DamageEffect damageEffect = player.GetComponentInChildren<DamageEffect>();
        if (damageEffect != null)
        {
            damageEffect.ShowDamageEffect();
            // damageEffect.PlayDamageSound();
        }
    }
    public virtual void Hit(MonoBehaviour target) { }

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
