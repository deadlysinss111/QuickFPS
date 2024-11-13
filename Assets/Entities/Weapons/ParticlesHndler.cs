using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesHndler : MonoBehaviour
{
    [SerializeField] protected ParticleSystem _canon;
    virtual public void Trigger()
    {
        _canon.Play();
    }

    virtual public void Stop()
    {
        _canon.Stop();
    }
}
