using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletBody : NetworkBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponent<Bullet>().Collide(collision);
    }
}
