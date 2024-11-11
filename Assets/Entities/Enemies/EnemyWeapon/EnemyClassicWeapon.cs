using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClassicWeapon : MonoBehaviour
{
    [SerializeField] GameObject _bulletPrefab;

    virtual public void Fire()
    {
        GameObject bullet = Instantiate(_bulletPrefab, transform.position + transform.rotation * new Vector3(2, 0, 0), transform.rotation);
        //Bullet bulletScript = bullet.GetComponent<Bullet>();
    }
}
