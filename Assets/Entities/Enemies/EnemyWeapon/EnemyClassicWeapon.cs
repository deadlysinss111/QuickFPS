using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyClassicWeapon : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _handSpot;

    virtual public void Fire()
    {
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + transform.rotation * new Vector3(2, 0, 0), transform.rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
        //Bullet bulletScript = bullet.GetComponent<Bullet>();
    }

    private void Update()
    {
        transform.position = _handSpot.position;
    }
}
