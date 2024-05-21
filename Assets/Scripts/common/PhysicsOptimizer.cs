using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsOptimizer : MonoBehaviour
{
    private void Awake()
    {
        OnBecameInvisible();
    }
    private void OnBecameVisible()
    {
        foreach (var collider in gameObject.GetComponents<Collider2D>())
        {
            collider.enabled = true;
        }
    }

    private void OnBecameInvisible()
    {
        foreach (var collider in gameObject.GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }
    }
}
