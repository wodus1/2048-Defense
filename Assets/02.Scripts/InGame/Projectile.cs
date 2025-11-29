using System;
using UnityEngine;

public class Projectile : MonoBehaviour //투사체 오브젝트
{
    private ProjectileSystem owner;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float duration = 0.5f;
    private float elapsed;
    private bool isMove = false;

    public void Initialize(ProjectileSystem owner)
    {
        this.owner = owner;
    }

    public void Shoot(Vector2 start, Vector2 target)
    {
        startPos = start;
        targetPos = target;
        transform.position = start;
        gameObject.SetActive(true);
        isMove = true;
    }

    private void Update()
    {
        if(!isMove)
            return;
        
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / duration);
        transform.position = Vector3.Lerp(startPos, targetPos, t);

        if (t >= 1f)
        {
            isMove = false;
            elapsed = 0f;
            owner.ReturnToPool(this);
        }
    }
}