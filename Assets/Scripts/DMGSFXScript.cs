using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DMGSFXScript : MonoBehaviour
{
    float timer;
    float maxTime;
    AnimationCurve curve;
    SpriteRenderer sr;
    Color ogColor;

    private void Start()
    {
        foreach(var dmg in gameObject.GetComponents<DMGSFXScript>())
        {
            if (dmg != this)
            {
                Destroy(this);
                return;
            }
        }


        curve = GameManager.Instance.dmgSFXCurve;
        maxTime = GameManager.Instance.dmgSFXDuration;
        timer = 0f;

        sr = GetComponent<SpriteRenderer>();
        ogColor = sr.color;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > maxTime)
        {
            sr.color = ogColor;
            Destroy(this);
            return;
        }

        float t = timer / maxTime;
        float e = curve.Evaluate(t);
        sr.color = Color.Lerp(ogColor, Color.red, e);
    }
}
