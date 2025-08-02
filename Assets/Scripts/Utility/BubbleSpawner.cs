using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : MonoBehaviour
{
    public float cd = 0.05f;
    float timer = 0f;
    public float range = 1f;

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnBubble();
            timer = cd;
        }
    }

    void SpawnBubble()
    {
        int randomIndex = Random.Range(0, CollectionManager.Instance.bubbles.Count);
        GameObject bubble = CollectionManager.Instance.bubbles[randomIndex];
        if (bubble != null)
        {
            float distance = Random.Range(0.8f, range);
            Vector2 randomPosition = Random.insideUnitCircle.normalized * distance;
            bubble = Instantiate(bubble,transform);
            bubble.transform.position = transform.position + new Vector3(randomPosition.x,randomPosition.y,0f);
            
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
