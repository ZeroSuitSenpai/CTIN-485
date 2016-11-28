using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TeleporterLogic : NetworkBehaviour
{

    public float moveSpeed = 3.0f;
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

    [ClientRpc]
    public void RpcSetPlayerOwner(GameObject player) {
        playerOwner = player;
    }

    protected void OnDisable() {
        if (playerOwner != null) {
            playerOwner.GetComponent<PlayerActionController>().teleporterInstance = null;
        }
    }
}
