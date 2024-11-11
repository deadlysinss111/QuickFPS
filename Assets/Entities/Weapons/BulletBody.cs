using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBody : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponent<Bullet>().Collide(collision);
    }
}
