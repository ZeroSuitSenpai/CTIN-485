using UnityEngine;
using UnityEngine.Networking;

public class PlayerActionController : NetworkBehaviour
{

    public Animator anim;

    float inputH;
    float inputV;

    public bool isLight;
    public bool isDark;

    public GameObject genericProjPrefab;
    public GameObject teleporterPrefab;

    public GameObject darkMelee;
    public GameObject lightMelee;
    public GameObject deathBall;

    public GameObject teleporterInstance;
    public Transform genericProjSpawn;

    void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        //input
        var x = Input.GetAxis("Horizontal") * Time.deltaTime * 200.0f;
        var z = Input.GetAxis("Vertical") * Time.deltaTime * 5.0f;

        inputH = Input.GetAxis("Horizontal");
        inputV = Input.GetAxis("Vertical");
        //animations  (networked)
        CmdUpdateAnimations(inputH, inputV);
        //movement
        transform.Rotate(0, x, 0);
        transform.Translate(0, 0, z);
        Vector3 movePos = transform.position;

        //bounds
        if (transform.position.z < 0.4) {
            movePos.z = 0.4f;
        }
        if (transform.position.z > 16) {
            movePos.z = 16;
        }
        if (transform.position.x < -13.35f) {
            movePos.x = -7f;
        }
        if (transform.position.x > 11) {
            movePos.x = 11;
        }
        transform.position = movePos;

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
            else {
                CmdServerTeleport(teleporterInstance);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            CmdDeathBall();
        }
    }

    void Start()
    {
        anim = GetComponent<Animator>();
        //if its server
        if (isServer)
        {
            //if youre the local player you are the server and therefore light unity chan
            if (isLocalPlayer) {
                isLight = true;
                gameObject.layer = LayerMask.NameToLayer("Light");
            }
            //Otherwise its dark unity chan
            else {
                isDark = true;
                gameObject.layer = LayerMask.NameToLayer("Dark");
                //Become dark
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers){
                    rend.material.color = Color.black;
                }
            }
        }
        else
        {
            //if youre the local player you are the client and therefore dark unity chan
            if (isLocalPlayer) {
                isDark = true;
                gameObject.layer = LayerMask.NameToLayer("Dark");
                //Become dark
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                foreach (Renderer rend in renderers) {
                    rend.material.color = Color.black;
                }
            }
            //Otherwise its light unity chan
            else {
                isLight = true;
                gameObject.layer = LayerMask.NameToLayer("Light");
            }
        }
    }

    [ClientRpc]
    void RpcUpdateAnimations(float h, float v) {
        anim.SetFloat("inputH", h);
        anim.SetFloat("inputV", v);
    }

    [Command]
    void CmdUpdateAnimations(float h, float v) {
        RpcUpdateAnimations(h, v);
    }

    [Command]
    void CmdFire()
    {
        if (base.isServer)
        {
            // Create generic projectile prefab
            GameObject genericProjectile = (GameObject)Instantiate(genericProjPrefab, genericProjSpawn.position, genericProjSpawn.rotation);
            //Get script
            GenericProjectileLogic gLogic = genericProjectile.GetComponent<GenericProjectileLogic>();
            // Make prefab move
            gLogic.velocity = genericProjectile.transform.forward * 4;
            //Set player owner
            gLogic.RpcSetPlayerOwner(gameObject);
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
            tLogic.velocity = teleporterInstance.transform.forward * 2;

            //Set its owner to be this object across the network
            tLogic.RpcSetPlayerOwner(gameObject);

            //Spawn it on the network
            NetworkServer.Spawn(teleporterInstance);

            //Set the instance on the network
            RpcSetTeleporter(teleporterInstance);

            // Destroy the bullet after 2 seconds
            Destroy(teleporterInstance.gameObject, 2.0f);
        }
    }

    [Command]
    void CmdDeathBall()
    {
        if (isDark)
        {
            deathBall = (GameObject)Instantiate(darkMelee, gameObject.transform.position, gameObject.transform.rotation);
        }
        else if (isLight)
        {
            deathBall = (GameObject)Instantiate(lightMelee, gameObject.transform.position, gameObject.transform.rotation);
        }
        NetworkServer.Spawn(deathBall);
        Destroy(deathBall, 0.1f);
    }

    [ClientRpc]
    void RpcSetTeleporter(GameObject teleporter) {
        teleporterInstance = teleporter;
    }

    [Command]
    void CmdServerTeleport(GameObject teleporter) {
        RpcClientTeleport(teleporter);
    }

    [ClientRpc]
    void RpcClientTeleport(GameObject teleporter) {
        Vector3 targetPos = teleporterInstance.transform.position;
        Destroy(teleporterInstance);
        teleporterInstance = null;
        transform.position = targetPos;
    }
}