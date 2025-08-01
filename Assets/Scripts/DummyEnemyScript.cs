using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyEnemyScript : MonoBehaviour
{
    PointOnPath pp;
    public float speed;
    public int HP;
    public int MoneyWorth;
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
        pp.MoveBy(speed * Time.deltaTime);
        transform.position = pp.GetPosition();
        if (pp.reachedEnd)
        {
            Destroy(gameObject);
            
            //TODO SFX
        }
    }

    public float GetPathDist() => pp.index + pp.t;

    public void TakeDamage(int damage)
    {
        HP -= damage;
        gameObject.AddComponent<DMGSFXScript>();
        if (HP <= 0)
        {
            Destroy(gameObject);
            //TODO give money
        }
    }
}
