using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DieInSeconds : MonoBehaviour
{
    public float timer = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Destroy(gameObject);
        }
    }
}
