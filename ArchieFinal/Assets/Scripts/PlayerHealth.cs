using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class PlayerHealth : NetworkBehaviour {

    public const int maxHealth = 100;

    [SyncVar(hook = "OnChangeHealth")]
    public int currentHealth = maxHealth;

    public RectTransform healthBar;

    public bool isDead;

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
    }

    public void TakeDamage(int damageAmt)
    {
        if (!isServer)
        {
            return;
        }

        currentHealth -= damageAmt;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            currentHealth = maxHealth;
            transform.position = new Vector3(500, 500, 500);
            RpcRespawn();
        }
    }

    void OnChangeHealth(int currentHealth)
    {
        healthBar.sizeDelta = new Vector2(currentHealth, healthBar.sizeDelta.y);
    }

    [ClientRpc]
    void RpcRespawn()
    {
        if (isLocalPlayer)
        {
            StartCoroutine(RespawnWithDelay(1));
        }
        ScoreManager gameScore = FindObjectOfType<ScoreManager>();
        //if you are light unity chan (server and local player or client and not local player)
        if ((isServer && isLocalPlayer) || (!isServer && !isLocalPlayer)) {
            //if the score manager exists
            if (gameScore != null) {
                //add score for dark unitychan
                gameScore.IncrementDarkScore();
            }
        }
        //else do it for dark
        else {
            //if the score manager exists
            if (gameScore != null) {
                //add score for light unitychan
                gameScore.IncrementLightScore();
            }
        }
    }

    [ClientRpc]
    public void RpcTakeDamage(int damage)
    {
        OnChangeHealth(damage);
    }

    [Command]
    public void CmdTakeDamage(int damage)
    {
        RpcTakeDamage(damage);
    }

     IEnumerator RespawnWithDelay(float delay)
    {
        if (isLocalPlayer)
        {
            yield return new WaitForSeconds(delay);
            transform.position = Vector3.zero;
        }
    }
}
