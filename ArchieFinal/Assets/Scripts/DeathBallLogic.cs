using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DeathBallLogic : NetworkBehaviour
{
    public GameObject playerOwner;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        var hit = other.gameObject;
        var healthScript = hit.GetComponent<PlayerHealth>();
        if (other.gameObject != playerOwner)
        {
            if (healthScript != null)
            {
                healthScript.TakeDamage(100);
            }
        }
    }

    [ClientRpc]
    public void RpcSetPlayerOwner(GameObject player)
    {
        playerOwner = player;
    }
}