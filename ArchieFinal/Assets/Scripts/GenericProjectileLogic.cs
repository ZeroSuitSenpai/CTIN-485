using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GenericProjectileLogic : NetworkBehaviour
{
    public float moveSpeed = 10.0f;
    public Vector3 velocity;
    public GameObject playerOwner;

    private void FixedUpdate()
    {
        // we want the bullet to be updated only on the server
        if (!base.isServer)
            return;

        // transform bullet on the server
        transform.position += velocity * Time.deltaTime * moveSpeed;
    }

    void OnCollisionEnter(Collision other)
    { 
        var hit = other.gameObject;
        var healthScript = hit.GetComponent<PlayerHealth>();
        if (healthScript != null && other.gameObject != playerOwner)
        {
            healthScript.TakeDamage(20);
        }
        Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcSetPlayerOwner(GameObject player)
    {
        playerOwner = player;
    }
}