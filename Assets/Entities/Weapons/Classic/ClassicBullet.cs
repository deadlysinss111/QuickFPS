using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Timeline;

public class ClassicBullet : Bullet
{
    [SerializeField] float _speed;
    protected override void Tick()
    {
        Vector3 direction = transform.rotation * new Vector3(1, 0, 0);
        transform.position += direction * _speed;
    }
}
