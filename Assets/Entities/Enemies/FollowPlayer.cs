using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private GameObject _player;
    [SerializeField] float _stopDistance = 8f;
    [SerializeField] float _shootDistance = 10f;
    [SerializeField] float _cooldown = 2f;
    float _remaingingCooldown;

    [SerializeField]EnemyClassicWeapon _weapon;
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.Find("Player");
    }

    void Update()
    {
        _remaingingCooldown -= Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
        Follow(distanceToPlayer);
        Shoot(distanceToPlayer);
    }
    void Follow(float distanceToPlayer)
    {
        

        if (distanceToPlayer > _stopDistance)
        {
            _agent.SetDestination(_player.transform.position);
        }
        else
        {
            _agent.ResetPath();
        }
    }

    void Shoot(float distanceToPlayer)
    {
        if(distanceToPlayer < _shootDistance && _remaingingCooldown <= 0)
        {
            _weapon.Fire();
            _remaingingCooldown = _cooldown;
        }
    }
}
