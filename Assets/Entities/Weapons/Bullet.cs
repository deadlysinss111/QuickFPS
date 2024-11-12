using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected float _speed = 0.1f;
    [SerializeField] protected float _lifeTime = 10;
    [SerializeField] float _damage = 10f;
    GameObject _effect;

    protected virtual void Tick() { }
    protected virtual void Hit(FollowPlayer enemy) {
        Hit((MonoBehaviour)enemy);
        enemy.GetComponent<FollowPlayer>().TakeDamage(_damage);
    }
    protected virtual void Hit(CharacterController player)
    {
        Hit((MonoBehaviour)player);
        player.TakeDamage(_damage);

        DamageEffect damageEffect = player.GetComponentInChildren<DamageEffect>();
        if (damageEffect != null)
        {
            damageEffect.ShowDamageEffect();
            // damageEffect.PlayDamageSound();
        }
    }

    protected virtual void Hit(MonoBehaviour target) { }

    protected void Update()
    {
        Tick();
        _lifeTime -= Time.deltaTime;
        if(_lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void Collide(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Weapon _) || collision.gameObject.layer == LayerMask.NameToLayer("Bullets"))
        {
            print("hit");
            return;
        }
        else if (collision.gameObject.TryGetComponent(out FollowPlayer enemy))
        {
            Hit(enemy);
        }
        else if (collision.gameObject.TryGetComponent(out CharacterController player))
        {
            Hit(player);
        }
        
        Destroy(gameObject);
    }
}
