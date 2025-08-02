using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyScript : RuntimeGO
{
    PointOnPath pp;
    public float speed;
    public int HP;
    public int MoneyWorth;

    float slowAmount = 0f;
    float slowDuration = 0f;
    // Start is called before the first frame update
    void Start()
    {
        pp = new PointOnPath();
        pp.index = 0;
        pp.t = 0f;
        pp.reachedEnd = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance != null && !GameManager.Instance.gameRunning)
        {
            Destroy(gameObject);
            return;
        }
        pp.MoveBy(speed * Time.deltaTime * (1-slowAmount));
        transform.position = pp.GetPosition();
        slowDuration -= Time.deltaTime;
        if (slowDuration <= 0f)
        {
            slowAmount = 0f;
        }
        if (pp.reachedEnd)
        {
            GameManager.Instance.TakePlayerDamage();
            Destroy(gameObject);
            
        }
    }

    public float GetPathDist() => pp.index + pp.t;

    public void TakeDamage(int damage, float slowAmount = 0.33f, float slowDuration = 0.2f)
    {
        HP -= damage;
        gameObject.AddComponent<DMGVFXScript>();
        if(slowAmount > this.slowAmount)
        {
            this.slowAmount = slowAmount;
            this.slowDuration = slowDuration;
        }
        else if(slowAmount == this.slowAmount)
        {
            this.slowDuration = Mathf.Max(this.slowDuration, slowDuration);
        }
        if (HP <= 0)
        {
            Destroy(gameObject);
            //TODO give money
        }
    }
}
