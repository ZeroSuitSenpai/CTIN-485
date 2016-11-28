using UnityEngine;
using UnityEngine.Networking;

public class PlayerActionController : NetworkBehaviour
{

    public GameObject genericProjPrefab;
    public Transform genericProjSpawn;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 150.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 3.0f;

        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdFire();
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    [Command]
    void CmdFire()
    {
        // Create generic projectile prefab
        GameObject genericProjectile = (GameObject)Instantiate(genericProjPrefab, genericProjSpawn.position,genericProjSpawn.rotation);

        // Make prefab move
        genericProjectile.GetComponent<Rigidbody>().velocity = genericProjectile.transform.forward * 6;

        //Spawn it on the network
        NetworkServer.Spawn(genericProjectile);

        // Destroy the bullet after 2 seconds
        Destroy(genericProjectile, 2.0f);

    }
}