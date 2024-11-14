using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletBody : NetworkBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Bullet bullet))
            return;
        transform.parent.GetComponent<Bullet>().Collide(collision);
        Destroy(gameObject);
    }
}
