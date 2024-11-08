using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    protected virtual void Tick() { }
    protected virtual void Hit() { }

    private void Update()
    {
        Tick();
    }
}
