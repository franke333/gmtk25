using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeGO : MonoBehaviour
{
    private void Awake()
    {
        GameManager.Instance.AddRuntimeGO(this);
    }

    private void OnDestroy()
    {
        if(!GameManager.Instance) return; // Check if GameManager is still available
        GameManager.Instance.RemoveRuntimeGO(this);
    }
}
