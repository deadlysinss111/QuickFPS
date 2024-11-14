using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private GameObject _player;
    [SerializeField] private float _health = 50f;
    [SerializeField] private float _stopDistance = 8f;
    [SerializeField] private float _shootDistance = 10f;
    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private float _randomMoveInterval = 10f;
    [SerializeField] private float _pauseDuration = 1f; // Durée de pause entre les mouvements
    private float _remainingCooldown;

    [SerializeField] private EnemyClassicWeapon _weapon;
    private Vector3 _randomDestination;

    private bool _isMoving = true;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _player = GameObject.FindWithTag("Player");

        if (_agent == null)
        {
            Debug.LogError("NavMeshAgent not found on the enemy.");
        }

        if (_player == null)
        {
            Debug.LogError("Player not found. Ensure the player object has the tag 'Player'.");
        }

        // S'assurer que l'agent commence sur le NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas))
        {
            _agent.Warp(hit.position);
            Debug.Log("Enemy placed on NavMesh at position: " + hit.position);
        }
        else
        {
            Debug.LogError("Enemy could not be placed on NavMesh.");
        }

        // Démarrer le premier déplacement aléatoire
        StartCoroutine(MoveAndPause());
    }

    IEnumerator MoveAndPause()
    {
        while (true)
        {
            if (_isMoving)
            {
                RandomMove();
                yield return new WaitForSeconds(_randomMoveInterval);
                _isMoving = false;
                _agent.ResetPath();
                yield return new WaitForSeconds(_pauseDuration);
                _isMoving = true;
            }
            else
            {
                yield return null;
            }
        }
    }

    void Update()
    {
        if (!_agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent is not on the NavMesh.");
            return;
        }

        _remainingCooldown -= Time.deltaTime;

        if (_isMoving)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, _player.transform.position);
            if (distanceToPlayer <= _stopDistance)
            {
                FollowPlayerAndShoot(distanceToPlayer);
            }
        }
    }

    void RandomMove()
    {
        Debug.Log("RandomMove called");
        _randomDestination = GetRandomDestination();
        if (_agent.SetDestination(_randomDestination))
        {
            Debug.Log("Random destination set: " + _randomDestination);
        }
        else
        {
            Debug.LogError("Failed to set random destination: " + _randomDestination);
        }
    }

    Vector3 GetRandomDestination()
    {
        Vector3 randomDirection = Random.insideUnitSphere * _stopDistance;
        randomDirection += transform.position;
        NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _stopDistance, NavMesh.AllAreas);
        return hit.position;
    }

    void FollowPlayerAndShoot(float distanceToPlayer)
    {
        if (distanceToPlayer <= _shootDistance && _remainingCooldown <= 0)
        {
            _weapon.Fire();
            Debug.Log("Shooting at player.");
            _remainingCooldown = _cooldown;
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
