using UnityEngine;
using UnityEngine.Networking;

public class PlayerActionController : NetworkBehaviour
{

    public GameObject genericProjPrefab;
    public GameObject teleporterPrefab;

    //wanna sync this one
    protected TeleporterLogic teleporterProjectile;

    public TeleporterLogic teleporterInstance;
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
            teleporterProjectile = (TeleporterLogic)Instantiate(teleporterPrefab, genericProjSpawn.position, genericProjSpawn.rotation) as TeleporterLogic;

            // Make prefab move
            teleporterProjectile.velocity = teleporterProjectile.transform.forward * 6;

            //Spawn it on the network
            NetworkServer.Spawn(teleporterProjectile.gameObject);

            //Call an rpc to set the teleporter instance everywhere
            RpcSetTeleporterInstance(teleporterProjectile.gameObject);

            // Destroy the bullet after 2 seconds
            Destroy(teleporterProjectile.gameObject, 4.0f);
        }
    }

    [ClientRpc]
    void RpcSetTeleporterInstance(GameObject teleporterPorjectile) {
        teleporterInstance = teleporterProjectile.GetComponent<TeleporterLogic>();
    }

    [Command]
    void CmdTranslocate()
    {
        if (base.isServer)
        {
            gameObject.transform.position = teleporterInstance.transform.position;
            Destroy(teleporterProjectile);
        }
    }
}