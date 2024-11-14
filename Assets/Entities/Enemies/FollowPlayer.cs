using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private GameObject _player;
    [SerializeField]  float _health = 50f;
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
            Vector3 pos = _player.transform.position;
            NavMesh.SamplePosition(pos, out NavMeshHit hit, 100, LayerMask.NameToLayer("All"));
            _agent.SetDestination(hit.position);
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

    public void TakeDamage(float damage, ulong dmgFrom)
    {
        _health -= damage;

        if (_health <= 0)
        {
            GameObject.Find("GameManager").GetComponent<GameManager>().IncrementScoreRpc(dmgFrom);
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died!");
        Destroy(gameObject);
    }
}
