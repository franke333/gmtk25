using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : RuntimeGO
{
    public float speed = 5f;
    public int damage = 1;
    public float lifetime = 5f;
    public Vector2 direction;
    public Sprite sprite;
    public Color color = Color.white;

    public float? slow, slowDuration;

    private float timer;
    private Rigidbody2D rb;
    private CircleCollider2D collider;
    private SpriteRenderer sr;

    bool dmgDealt = false;
    

    public void Initialize(Vector2 direction, float speed, float lifetime, int damage = 1, Sprite sprite = null, float? slow = null, float? slowDuration = null, Color? color = null)
    {      
        this.direction = direction;
        this.speed = speed;
        this.lifetime = lifetime;
        this.damage = damage;
        this.sprite = sprite;

        this.slow = slow;
        this.slowDuration = slowDuration;
        if (color.HasValue) this.color = color.Value;
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
        sr.color = color;
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
        if (dmgDealt)
            return;
        if (collision.CompareTag("Enemy"))
        {
            DummyEnemyScript enemy = collision.GetComponent<DummyEnemyScript>();
            if (enemy != null && enemy.HP > 0)
            {
                if( slow.HasValue && slowDuration.HasValue)
                {
                    enemy.TakeDamage(damage, slow.Value, slowDuration.Value);
                }
                else
                {
                    enemy.TakeDamage(damage);
                }
                dmgDealt = true;
                Destroy(gameObject);
            }
        }
    }


}
