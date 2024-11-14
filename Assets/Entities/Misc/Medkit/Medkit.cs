using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    [NonSerialized] public ItemSpawner _spawnerRef;

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.TryGetComponent(out CharacterController player))
        {
            print("gatcha");
            player.HealSelf();
            Destroy(gameObject);
            _spawnerRef.Borrow();
        }
    }
}
