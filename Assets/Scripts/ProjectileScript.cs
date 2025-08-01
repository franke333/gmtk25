using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public float lifetime = 5f;
    public Vector2 direction;
    public Sprite sprite;

    private float timer;
    private Rigidbody2D rb;
    private CircleCollider2D collider;
    private SpriteRenderer sr;

    public void Initialize(Vector2 direction, float speed, float lifetime, int damage = 1, Sprite sprite = null)
    {      
        this.direction = direction;
        this.speed = speed;
        this.lifetime = lifetime;
        this.damage = damage;
        this.sprite = sprite;
    }

    private void Start()
    {
        rb = gameObject.AddComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.velocity = direction.normalized * speed;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        collider = gameObject.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = 0.1f;
        timer = lifetime;

        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            DummyEnemyScript enemy = collision.GetComponent<DummyEnemyScript>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }


}
