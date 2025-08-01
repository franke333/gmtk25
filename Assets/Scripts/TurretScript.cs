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

public class TurretScript : MonoBehaviour
{

    private List<DummyEnemyScript> enemies = new List<DummyEnemyScript>();
    public float cd = 1f;
    private float timer = 0f;

    [SerializeField] Sprite DEBUGSPRITE;


    private void Update()
    {
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
        SpawnBullet(target.transform.position - transform.position, 5f, 2f, 1);
        timer = cd;
        
        
    }

    private void SpawnBullet(Vector2 direction, float speed, float lifetime, int damage = 1)
    {
        new GameObject("Bullet")
        {
            transform =
            {
                position = transform.position,
                rotation = Quaternion.LookRotation(Vector3.forward, direction)
            }
        }.AddComponent<ProjectileScript>().Initialize(direction, speed, lifetime, damage, DEBUGSPRITE);
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
