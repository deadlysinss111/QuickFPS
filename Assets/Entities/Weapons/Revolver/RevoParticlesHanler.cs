using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevoParticlesHanler : ParticlesHndler
{
    private Coroutine _coroutine = null;
    public override void Trigger()
    {
        base.Trigger();
        if(_coroutine != null )
            StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(StopSmkeCoroutine());
    }

    private IEnumerator StopSmkeCoroutine()
    {
        yield return new WaitForSeconds(2);

        base.Stop();
    }
}
