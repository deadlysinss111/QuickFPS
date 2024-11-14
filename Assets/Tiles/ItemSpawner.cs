using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class ItemSpawner : NetworkBehaviour
{
    [SerializeField] GameObject _prefab;
    [SerializeField] float _delay;
    float _remainingDelay = 2;
    bool _isBusy = false;

    private void Update()
    {
        if(_isBusy || !IsHost) return;

        _remainingDelay -= Time.deltaTime;
        if(_remainingDelay <= 0)
        {
            _isBusy = true;
            GameObject instance = Instantiate(_prefab, transform.position + new Vector3(0, 1, 0), transform.rotation);
            instance.GetComponent<NetworkObject>().Spawn();

            if(instance.TryGetComponent(out Weapon weapon))
            {
                weapon._spawnerRef = this;
            }
            else if (instance.TryGetComponent(out Medkit medkit))
            {
                medkit._spawnerRef = this;
            }

            _isBusy = true;
            _remainingDelay = _delay;
        }
    }

    public void Borrow()
    {
        _isBusy = false;
    }
}