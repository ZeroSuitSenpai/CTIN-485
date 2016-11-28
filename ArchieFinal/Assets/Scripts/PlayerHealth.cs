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
