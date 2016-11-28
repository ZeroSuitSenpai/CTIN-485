using UnityEngine;
using UnityEngine.Networking;

public class PlayerActionController : NetworkBehaviour
{

    public GameObject genericProjPrefab;
    public GameObject teleporterPrefab;

    GameObject teleporterProjectile;

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
            if (teleporterInstance == null)
            {
                CmdTeleporter();
            }
            else
            {
                CmdTranslocate();
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
            teleporterProjectile = (GameObject)Instantiate(teleporterPrefab, genericProjSpawn.position, genericProjSpawn.rotation);

            // Make prefab move
            teleporterProjectile.GetComponent<TeleporterLogic>().velocity = teleporterProjectile.transform.forward * 6;

            //Spawn it on the network
            NetworkServer.Spawn(teleporterProjectile);

            // Destroy the bullet after 2 seconds
            Destroy(teleporterProjectile, 4.0f);
        }
        if (isLocalPlayer)
        {
            teleporterInstance = teleporterProjectile;
        }
    }

    [Command]
    void CmdTranslocate()
    {
        if (base.isServer)
        {
            gameObject.transform.position = teleporterProjectile.transform.position;
            Destroy(teleporterProjectile);
        }
    }
}