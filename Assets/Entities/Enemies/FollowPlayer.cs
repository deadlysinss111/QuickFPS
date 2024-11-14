using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : MonoBehaviour
{
    private NavMeshAgent _agent;
    private GameObject _player;
    [SerializeField] private float _maxHealth = 50f;
    private float _health;
    [SerializeField] private float _stopDistance = 8f;
    [SerializeField] private float _shootDistance = 10f;
    [SerializeField] private float _cooldown = 2f;
    [SerializeField] private float _randomMoveInterval = 10f;
    [SerializeField] private float _pauseDuration = 1f; // Dur�e de pause entre les mouvements
    private float _remainingCooldown;

    bool _isDead = false;

    [SerializeField] private EnemyClassicWeapon _weapon;
    private Vector3 _randomDestination;

    private bool _isMoving = true;

    void Start()
    {
        _health = _maxHealth;
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

        // D�marrer le premier d�placement al�atoire
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

        if (_health <= 0 && _isDead == false)
        {
            Die(dmgFrom);
        }
    }

    private void Die(ulong from)
    {
        _isDead = true;
        GameObject.Find("GameManager").GetComponent<GameManager>().IncrementScoreRpc(from);
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine()
    {
        transform.position = new Vector3(500, 500, 500);
        yield return new WaitForSeconds(4);
        _isDead = false;
        GameObject.Find("GameManager").GetComponent<GameManager>().RespawnEnemy(gameObject);
    }

    public void Heal()
    {
        _health = _maxHealth;
    }
}
