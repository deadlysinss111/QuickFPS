using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEffectDecay : MonoBehaviour
{
    [SerializeField] float _decayAfter = 3;
    [SerializeField] float _decayDuring = 2;
    float _decayTime;

    Action _behave;

    private void Awake()
    {
        _behave = WaitForDeCaying;
        _decayTime = _decayDuring;
    }

    private void Update()
    {
        _behave();
    }

    private void WaitForDeCaying()
    {
        _decayAfter -= Time.deltaTime;
        if( _decayAfter <= 0 )
        {
            _behave = Decay;
        }
    }

    private void Decay()
    {
        _decayDuring -= Time.deltaTime;
        if(_decayDuring <= 0)
        {
            Destroy(gameObject);
        }
        else
        {
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            Color col = renderer.color;
            col.a = _decayDuring / _decayTime;
            renderer.color = col;
        }
    }
}
