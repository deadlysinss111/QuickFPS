using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatergunBullet : Bullet
{
    protected override void Tick()
    {
        Vector3 direction = transform.rotation * new Vector3(1, 0, 0);
        transform.position += direction * _speed;
    }
}
