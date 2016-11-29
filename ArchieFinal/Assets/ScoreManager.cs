using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class ScoreManager : MonoBehaviour {
    public Text lightScore;
    public Text darkScore;

    private int lightScoreValue = 0;
    private int darkScoreValue = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    //if either player has 10 points end the game
        if (lightScoreValue > 9 || darkScoreValue > 9) {
            EndGame();
        }
	}

    public void IncrementLightScore() {
        lightScoreValue++;
        lightScore.text = lightScoreValue.ToString();
    }
    public void IncrementDarkScore() {
        darkScoreValue++;
        darkScore.text = darkScoreValue.ToString();
    }
    public void EndGame() {
        //quit and return
        Destroy(FindObjectOfType<NetworkLobbyManager>().gameObject);
        SceneManager.LoadScene(0);
    }
}
