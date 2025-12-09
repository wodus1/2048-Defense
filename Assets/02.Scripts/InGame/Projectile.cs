using System;
using UnityEngine;

public class Projectile : MonoBehaviour //투사체 오브젝트
{
    private ProjectileSystem owner;
    private Vector3 startPos;
    private Vector3 direction;
    private float speed;
    private float damage;
    
    public float Damage => damage;

    public void Initialize(ProjectileSystem owner)
    {
        this.owner = owner;
    }

    public void Shoot(Vector2 start, Vector2 target, float damage, float speed)
    {
        transform.position = start;
        startPos = start;
        direction = (target - start).normalized;
        this.damage = damage;
        this.speed = speed;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (owner.IsPause())
            return;

        transform.position += direction * speed * Time.deltaTime;

        if(Vector3.Distance(startPos, transform.position) > 1500)
        {
            owner.ReturnToPool(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Monster"))
        {
            owner.ReturnToPool(this);
        }
    }
}