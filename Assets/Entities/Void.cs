using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Void : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out CharacterController player))
        {
            player.TakeDamage(10000, 100);
        }
        else
            Destroy(collision.gameObject);
    }
}
