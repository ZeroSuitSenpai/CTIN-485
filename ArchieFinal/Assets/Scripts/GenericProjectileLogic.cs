using UnityEngine;
using System.Collections;

public class GenericProjectileLogic : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        var hit = other.gameObject;
        var healthScript = hit.GetComponent<PlayerHealth>();
        if (healthScript != null)
        {
            healthScript.TakeDamage(20);
        }
        Destroy(gameObject);
    }
}