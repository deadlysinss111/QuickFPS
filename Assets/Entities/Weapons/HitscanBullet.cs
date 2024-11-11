using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitscanBullet : MonoBehaviour
{
    [SerializeField] int _damage = 10;
    [SerializeField] int _dmgFalloff = 0;
    [SerializeField] GameObject _hitEffect;

    public void Trigger(RaycastHit hit)
    {
        //if (hit.transform.TryGetComponent(out Weapon _))
        //{
        //    print("hit");
        //    return;
        //}
        if (hit.transform.TryGetComponent(out FollowPlayer enemy))
        {
            //Hit(enemy);
        }
        else if (hit.transform.TryGetComponent(out CharacterController player))
        {
            //Hit(player);
        }

        HitEffect(hit);
        Destroy(gameObject);
    }

    virtual protected void HitEffect(RaycastHit hit)
    {
        GameObject hole = Instantiate(_hitEffect, hit.point + Vector3.Normalize(hit.normal)*0.01f, Quaternion.LookRotation(hit.normal));
        hole.transform.SetParent(hit.transform);
    }
}
