using UnityEngine;
using UnityEngine.Networking;

public class PlayerActionController : NetworkBehaviour
{

    public GameObject genericProjPrefab;
    public GameObject teleporterPrefab;

    public GameObject teleporterInstance;
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

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //If theres no puck orb shoot a puck orb
            if (teleporterInstance == null)
            {
                CmdTeleporter();
            }
        }
    }

    public override void OnStartLocalPlayer()
    {
        GetComponent<MeshRenderer>().material.color = Color.blue;
    }

    [Command]
    void CmdFire()
    {
        if (base.isServer)
        {
            // Create generic projectile prefab
            GameObject genericProjectile = (GameObject)Instantiate(genericProjPrefab, genericProjSpawn.position, genericProjSpawn.rotation);
            // Make prefab move
            genericProjectile.GetComponent<GenericProjectileLogic>().velocity = genericProjectile.transform.forward * 6;
            // Destroy the bullet after 2 seconds
            Destroy(genericProjectile, 2.0f);
            //Spawn it on the network
            NetworkServer.Spawn(genericProjectile);
        }
    }

    [Command]
    void CmdTeleporter()
    {
        if (base.isServer)
        {
            // Create generic projectile prefab
            teleporterInstance = (GameObject)Instantiate(teleporterPrefab, genericProjSpawn.position, genericProjSpawn.rotation);

            //Get the script we want
            TeleporterLogic tLogic = teleporterInstance.GetComponent<TeleporterLogic>();

            // Make prefab move
            tLogic.GetComponent<TeleporterLogic>().velocity = teleporterInstance.transform.forward * 6;

            //Set its owner to be this object across the network
            tLogic.RpcSetPlayerOwner(gameObject);

            //Spawn it on the network
            NetworkServer.Spawn(teleporterInstance);

            // Destroy the bullet after 4 seconds
            Destroy(teleporterInstance.gameObject, 4.0f);
        }
    }
}