using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TurretType
{
    Aoe,
    Slow,
    RapidFire,
    Shotgun

}

public class TurretScript : RuntimeGO
{

    private List<DummyEnemyScript> enemies = new List<DummyEnemyScript>();
    public float cd = 1f;
    private float timer = 0f;
    public TurretType turretType = TurretType.Aoe;
    public int level = 0;
    public float range = 3f;

    Color color;

    private void Start()
    {
        //get range
        switch (turretType)
        {
            case TurretType.Aoe:
                range = CollectionManager.Instance.aoeturretLevels[level].range;
                gameObject.GetComponent<BubbleSpawner>().range = range;
                break;
            case TurretType.Slow:
                range = CollectionManager.Instance.slowturretLevels[level].range;
                break;
            case TurretType.RapidFire:
                range = CollectionManager.Instance.rapidfireturretLevels[level].range;
                break;
            case TurretType.Shotgun:
                range = CollectionManager.Instance.shotgunturretLevels[level].range;
                break;
            default:
                break;
        }
        gameObject.GetComponent<CircleCollider2D>().radius = range;
        color = gameObject.GetComponent<SpriteRenderer>().color;

    }

    private void Update()
    {
        if (!GameManager.Instance.gameRunning)
        {
            Destroy(gameObject);
            return;
        }
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Shoot();
        }
    }
    private void Shoot()
    {
        DummyEnemyScript target = GetTarget();
        if (target == null)
            return;

        switch (turretType)
        {
            case TurretType.Aoe:
                ShootAOE();
                break;
            case TurretType.Slow:
                ShootSlow(target);
                break;
            case TurretType.RapidFire:
                ShootRapidFire(target);
                break;
            case TurretType.Shotgun:
                ShootShotgun(target);
                break;
            default:
                break;
        }
    }

    private void ShootAOE()
    {
        TurretLevel levelData = CollectionManager.Instance.aoeturretLevels[level];
        // iterate over indices
        int count = enemies.Count;
        for (int i = 0; i < count; i++)
        {
            DummyEnemyScript enemy = enemies[i];
            enemy.TakeDamage(levelData.damage);
            if(count > enemies.Count) // Check if the list has changed during iteration
            {
                count = enemies.Count;
                i--; // Adjust index since we removed an enemy
            }
        }
        timer = levelData.cd;
    }

    private void ShootRapidFire(DummyEnemyScript target)
    {
        TurretLevel levelData = CollectionManager.Instance.rapidfireturretLevels[level];
        Vector2 direction = (target.transform.position - transform.position).normalized;
        Sprite spr = CollectionManager.Instance.generalProjectiles[Random.Range(0, CollectionManager.Instance.generalProjectiles.Count)];
        SpawnBullet(direction, 6.5f, 1.5f, levelData.damage,sprite: spr);
        timer = levelData.cd;
    }

    private void ShootShotgun(DummyEnemyScript target)
    {
        ShotgunLevel levelData = CollectionManager.Instance.shotgunturretLevels[level];
        Vector2 direction = (target.transform.position - transform.position).normalized;
        for (int i = 0; i < levelData.pellets; i++)
        {
            float angle = Random.Range(-15f, 15f);
            Vector2 spreadDirection = Quaternion.Euler(0, 0, angle) * direction;
            Sprite spr = CollectionManager.Instance.generalProjectiles[Random.Range(0, CollectionManager.Instance.generalProjectiles.Count)];
            SpawnBullet(spreadDirection, 4f, 2f, levelData.damage,sprite: spr);
        }
        timer = levelData.cd;
    }

    private void ShootSlow(DummyEnemyScript target)
    {
        SlowLevel levelData = CollectionManager.Instance.slowturretLevels[level];
        Vector2 direction = (target.transform.position - transform.position).normalized;
        SpawnBullet(direction, 3.5f, 1.7f, levelData.damage, levelData.slowAmount, 1f,CollectionManager.Instance.slowProjectile);
        timer = levelData.cd;
    }

    private void SpawnBullet(Vector2 direction, float speed, float lifetime, int damage = 1, float? slowAmount = null, float? slowDuration = null, Sprite sprite = null)
    {
        new GameObject("Bullet")
        {
            transform =
            {
                position = transform.position,
                rotation = Quaternion.LookRotation(Vector3.forward, direction)
            }
        }.AddComponent<ProjectileScript>().Initialize(direction, speed, lifetime, damage, sprite, slowAmount,slowDuration, color);
    }

    private DummyEnemyScript GetTarget()
    {
        if (enemies.Count == 0)
            return null;
        DummyEnemyScript target = enemies[0];
        foreach (DummyEnemyScript enemy in enemies)
        {
            if (enemy.GetPathDist() > target.GetPathDist())
            {
                target = enemy;
            }
        }
        return target;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DummyEnemyScript enemy = collision.gameObject.GetComponent<DummyEnemyScript>();
            if (enemy != null)
            {
                enemies.Add(enemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Collision ended with: " + collision.gameObject.name);
        if (collision.gameObject.CompareTag("Enemy"))
        {
            DummyEnemyScript enemy = collision.gameObject.GetComponent<DummyEnemyScript>();
            if (enemy != null)
            {
                enemies.Remove(enemy);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (DummyEnemyScript enemy in enemies)
        {
            if (enemy != null)
            {
                Gizmos.DrawLine(transform.position, enemy.transform.position);
            }
        }
    }
}
